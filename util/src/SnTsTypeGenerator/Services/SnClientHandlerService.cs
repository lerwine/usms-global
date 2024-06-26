using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static SnTsTypeGenerator.Services.SnApiConstants;
using static SnTsTypeGenerator.Services.CmdLineConstants;

namespace SnTsTypeGenerator.Services;

public sealed class SnClientHandlerService
{
    private readonly ILogger<SnClientHandlerService> _logger;
    private readonly RemoteUriService _remoteUri;

    private SnAccessToken? _token;

    public NetworkCredential? ClientCredentials { get; }

    public NetworkCredential UserCredentials { get; }

    /// <summary>
    /// Indicates whether service initialization was successful.
    /// </summary>
    internal bool InitSuccessful => _remoteUri.InitSuccessful && UserCredentials is not null;

    private async Task<JsonNode?> GetJsonResponseAsync(HttpClientHandler handler, HttpRequestMessage message, Uri requestUri, CancellationToken cancellationToken)
    {
        using HttpClient httpClient = new(handler);
        _logger.LogAPIRequestStart(requestUri);
        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException exception)
        {
            string? text;
            try { text = await response.Content.ReadAsStringAsync(cancellationToken); }
            //codeql[cs/catch-of-all-exceptions] No need to record exception since there may not be string content.
            catch { text = null; }
            throw new RequestFailedException(requestUri, exception, text);
        }
        string responseBody;
        try { responseBody = await response.Content.ReadAsStringAsync(cancellationToken); }
        //codeql[cs/catch-of-all-exceptions] Won't fix - API documentation doesn't indicate what type of exception may be thrown.
        catch (Exception exception)
        {
            throw new GetResponseContentFailedException(requestUri, exception);
        }
        cancellationToken.ThrowIfCancellationRequested();
        string? mediaType = response.Content.Headers.ContentType?.MediaType;
        if (!string.IsNullOrEmpty(mediaType) && mediaType != MediaTypeNames.Application.Json)
            throw new InvalidHttpResponseException(requestUri, responseBody, mediaType);
        else if (string.IsNullOrWhiteSpace(responseBody))
            throw new InvalidHttpResponseException(requestUri, responseBody);
        JsonNode? result;
        try { result = JsonNode.Parse(responseBody); }
        catch (JsonException exception) { throw new ResponseParsingException(requestUri, responseBody, exception); }
        _logger.LogAPIRequestCompleted(requestUri, result);
        return result;
    }

    private async Task<JsonNode?> GetJsonAsync(HttpClientHandler handler, Uri requestUri, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
    {
        using HttpRequestMessage message = new(HttpMethod.Get, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        if (ClientCredentials is not null)
            message.Headers.Add(HEADER_KEY_ACCESS_TOKEN, (await GetAccessTokenAsync(cancellationToken)).AccessToken);
        configureHeaders?.Invoke(message.Headers);
        return await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
    }

    private async Task<JsonNode?> PostJsonAsync(HttpClientHandler handler, Uri requestUri, JsonNode? content, Action<HttpRequestHeaders>? configureHeaders, CancellationToken cancellationToken)
    {
        using HttpClient httpClient = new(handler);
        using HttpRequestMessage message = new(HttpMethod.Post, requestUri);
        message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
        if (ClientCredentials is not null)
            message.Headers.Add(HEADER_KEY_ACCESS_TOKEN, (await GetAccessTokenAsync(cancellationToken)).AccessToken);
        configureHeaders?.Invoke(message.Headers);
        if (content is not null)
            message.Content = JsonContent.Create(content, new MediaTypeHeaderValue(MediaTypeNames.Application.Json));
        return await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
    }

    internal HttpClientHandler CreateHttpClientHandler()
    {
        if (!InitSuccessful)
            throw new InvalidOperationException();
        // return (ClientCredentials is null) ? new() { Credentials = UserCredentials } : new();
        return new() { Credentials = UserCredentials };
    }

    /// <summary>
    /// Gets a ServiceNow access token.
    /// </summary>
    /// <param name="cancellationToken">The token to observe.</param>
    /// <returns>The access token.</returns>
    /// <seealso href="https://docs.servicenow.com/en-US/bundle/vancouver-platform-security/page/administer/security/reference/r_OAuthAPIResponseParameters.html"/>
    internal async Task<SnAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful || ClientCredentials is null)
            throw new InvalidOperationException();
        SnAccessToken? token = _token;
        JsonNode? jsonNode;
        DateTime createdOn;
        Uri requestUri;
        if (token is null)
        {
            requestUri = _remoteUri.GetUrl(URI_PATH_AUTH_TOKEN);
            using var getTokenScope = _logger.BeginActivityScope(LogActivityType.GetAccessToken, new JsonObject()
            {
                { nameof(HttpRequestMessage.RequestUri), JsonValue.Create(requestUri.AbsoluteUri) },
                { HEADER_KEY_CLIENT_ID, JsonValue.Create(ClientCredentials.UserName) },
                { HEADER_KEY_USERNAME, JsonValue.Create(UserCredentials.UserName) }
            });

            createdOn = DateTime.Now;
            using HttpClientHandler handler = CreateHttpClientHandler();
            using HttpClient httpClient = new(handler);
            using HttpRequestMessage message = new(HttpMethod.Post, requestUri)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new(HEADER_KEY_GRANT_TYPE, HEADER_KEY_PASSWORD),
                    new(HEADER_KEY_CLIENT_ID, ClientCredentials.UserName),
                    new(HEADER_KEY_CLIENT_SECRET, ClientCredentials.Password),
                    new(HEADER_KEY_USERNAME, UserCredentials.UserName),
                    new(HEADER_KEY_PASSWORD, UserCredentials.Password)
                })
            };
            message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            jsonNode = await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
        }
        else
        {
            if (token.ExpiresOn > DateTime.Now)
                return token;
            requestUri = _remoteUri.GetUrl(URI_PATH_AUTH_TOKEN);
            using var refreshTokenScope = _logger.BeginActivityScope(LogActivityType.RefreshAccessToken, new JsonObject()
            {
                { nameof(HttpRequestMessage.RequestUri), JsonValue.Create(requestUri.AbsoluteUri) },
                { HEADER_KEY_CLIENT_ID, JsonValue.Create(ClientCredentials.UserName) },
                { HEADER_KEY_REFRESH_TOKEN, JsonValue.Create(token.RefreshToken) }
            });
            createdOn = DateTime.Now;
            using HttpClientHandler handler = CreateHttpClientHandler();
            using HttpClient httpClient = new(handler);
            using HttpRequestMessage message = new(HttpMethod.Post, requestUri)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new(HEADER_KEY_GRANT_TYPE, HEADER_KEY_REFRESH_TOKEN),
                    new(HEADER_KEY_CLIENT_ID, ClientCredentials.UserName),
                    new(HEADER_KEY_CLIENT_SECRET, ClientCredentials.Password),
                    new(HEADER_KEY_REFRESH_TOKEN, token.RefreshToken)
                })
            };
            message.Headers.Add(HEADER_KEY_ACCEPT, MediaTypeNames.Application.Json);
            jsonNode = await GetJsonResponseAsync(handler, message, requestUri, cancellationToken);
        }
        if (jsonNode is not JsonObject resultObj)
            throw new InvalidResponseTypeException(requestUri, jsonNode);
        if (resultObj.TryCoercePropertyAsNonEmpty(JSON_KEY_ACCESS_TOKEN, out string? access_token) && resultObj.TryCoercePropertyAsNonEmpty(JSON_KEY_REFRESH_TOKEN, out string? refresh_token) && resultObj.TryCoercePropertyAsInt("expires_in", out int expires_in))
        {
            _token = token = new(access_token, refresh_token, createdOn.AddSeconds(expires_in));
            return token;
        }
        throw new InvalidHttpResponseException(requestUri, resultObj.ToJsonString());
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, string query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path, query);
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, string query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path, query);
        return (requestUri, await GetJsonAsync(handler, requestUri, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(string path, string query, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path, query);
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> GetJsonAsync(HttpClientHandler handler, string path, string query, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path, query);
        return (requestUri, await GetJsonAsync(handler, requestUri, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, null, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, JsonNode content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, content, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, null, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, JsonNode content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, content, null, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, null, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(string path, JsonNode content, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using HttpClientHandler handler = CreateHttpClientHandler();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, content, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, null, configureHeaders, cancellationToken));
    }

    internal async Task<(Uri RequestUri, JsonNode? Response)> PostJsonAsync(HttpClientHandler handler, string path, JsonNode content, Action<HttpRequestHeaders> configureHeaders, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!InitSuccessful)
            throw new InvalidOperationException();
        Uri requestUri = _remoteUri.GetUrl(path);
        return (requestUri, await PostJsonAsync(handler, requestUri, content, configureHeaders, cancellationToken));
    }

    public SnClientHandlerService(IOptions<AppSettings> appSettingsOptions, RemoteUriService remoteUriService, ILogger<SnClientHandlerService> logger)
    {
        _logger = logger;
        _remoteUri = remoteUriService;
        AppSettings appSettings = appSettingsOptions.Value;
        if (appSettings.ShowHelp() || !_remoteUri.InitSuccessful)
        {
            UserCredentials = null!;
            return;
        }
        string? clientId = appSettings.ClientId;
        string? clientSecret = appSettings.ClientSecret;
        if (clientId is null)
        {
            if (clientSecret is not null)
                _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.ClientId));
        }
        else
        {
            if (clientSecret is null)
            {
                Console.Write("Client Secret: ");
                clientSecret = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.ClientSecret));
                    UserCredentials = null!;
                    return;
                }
            }
            ClientCredentials = new(clientId, clientSecret);
        }
        string? userName = appSettings.UserName;
        if (userName is null)
        {
            Console.Write("User Name: ");
            userName = Console.ReadLine();
            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.UserName), SHORTHAND_u);
                UserCredentials = null!;
                return;
            }
        }
        string? password = appSettings.Password;
        if (password is null)
        {
            Console.Write("Password: ");
            password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogCriticalSettingValueNotProvided(nameof(AppSettings.Password), SHORTHAND_p);
                UserCredentials = null!;
                return;
            }
        }
        UserCredentials = new NetworkCredential(userName, password);
    }
}

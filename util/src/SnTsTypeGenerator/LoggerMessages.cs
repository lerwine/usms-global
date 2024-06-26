using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using static SnTsTypeGenerator.Services.SnApiConstants;
using static SnTsTypeGenerator.Services.CmdLineConstants;
using SnTsTypeGenerator.Services;
using System.Data.Common;
using System.Data;

namespace SnTsTypeGenerator;
public static class LoggerMessages
{
    /// <summary>
    /// Indicates whether an exception has been logged.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="exception">The exception.</param>
    /// <returns><see langword="true"/> if the <paramref name="exception"/> does not implement <see cref="ILogTrackable"/>;
    /// otherwise, this will log the exception if <see cref="ILogTrackable.IsLogged"/> is <see langword="false"/>.</returns>
    public static bool IsNotLogged(this ILogger logger, Exception exception)
    {
        if (exception is ILogTrackable logTrackable)
        {
            if (!logTrackable.IsLogged)
                logTrackable.Log(logger);
            return false;
        }
        return true;
    }

    public static void LogIfThrown(this ILogger logger, Action action)
    {
        try { action(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception);
            throw new TrackedException(exception);
        }
    }

    public static async Task LogIfThrownAsync(this ILogger logger, Func<Task> asyncAction)
    {
        try { await asyncAction(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception);
            throw new TrackedException(exception);
        }
    }

    public static T LogIfThrown<T>(this ILogger logger, Func<T> func)
    {
        try { return func(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception);
            throw new TrackedException(exception);
        }
    }

    public static async Task<T> LogIfThrownAsync<T>(this ILogger logger, Func<Task<T>> asyncFunc)
    {
        try { return await asyncFunc(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception);
            throw new TrackedException(exception);
        }
    }

    public static void LogOrThrowIfNotTrackable(this ILogger logger, Exception exception)
    {
        if (exception is ILogTrackable logTrackable)
        {
            if (!logTrackable.IsLogged)
                logTrackable.Log(logger);
        }
        else
        {
            logger.LogUnexpectedException(exception);
            throw new TrackedException(exception);
        }
    }

    public static void WithActivityScope(this ILogger logger, LogActivityType type, Action action)
    {
        using var scope = logger.BeginActivityScope(type);
        try { action(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task WithActivityScopeAsync(this ILogger logger, LogActivityType type, Func<Task> asyncAction)
    {
        using var scope = logger.BeginActivityScope(type);
        try { await asyncAction(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static void WithActivityScope(this ILogger logger, LogActivityType type, string context, Action action)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { action(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task WithActivityScopeAsync(this ILogger logger, LogActivityType type, string context, Func<Task> asyncAction)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { await asyncAction(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static void WithActivityScope(this ILogger logger, LogActivityType type, JsonNode context, Action action)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { action(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task WithActivityScopeAsync(this ILogger logger, LogActivityType type, JsonNode context, Func<Task> asyncAction)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { await asyncAction(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static T WithActivityScope<T>(this ILogger logger, LogActivityType type, Func<T> func)
    {
        using var scope = logger.BeginActivityScope(type);
        try { return func(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task<T> WithActivityScopeAsync<T>(this ILogger logger, LogActivityType type, Func<Task<T>> asyncFunc)
    {
        using var scope = logger.BeginActivityScope(type);
        try { return await asyncFunc(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static T WithActivityScope<T>(this ILogger logger, LogActivityType type, string context, Func<T> func)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { return func(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task<T> WithActivityScopeAsync<T>(this ILogger logger, LogActivityType type, string context, Func<Task<T>> asyncFunc)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { return await asyncFunc(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static T WithActivityScope<T>(this ILogger logger, LogActivityType type, JsonNode context, Func<T> func)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { return func(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    public static async Task<T> WithActivityScopeAsync<T>(this ILogger logger, LogActivityType type, JsonNode context, Func<Task<T>> asyncFunc)
    {
        using var scope = logger.BeginActivityScope(type, context);
        try { return await asyncFunc(); }
        catch (Exception exception)
        {
            if (exception is ILogTrackable logTrackable)
            {
                if (!logTrackable.IsLogged)
                    logTrackable.Log(logger);
                throw;
            }
            logger.LogUnexpectedException(exception, type.ToString("F").Replace("_", " "));
            throw new TrackedException(exception);
        }
    }

    #region Activity Scope

    private static readonly Func<ILogger, LogActivityType, IDisposable?> _activityScope1 = LoggerMessage.DefineScope<LogActivityType>("Activity: {Activity}");

    private static readonly Func<ILogger, LogActivityType, string, IDisposable?> _activityScope2 = LoggerMessage.DefineScope<LogActivityType, string>("Activity: {Activity}; Context: {Context}");

    /// <summary>
    /// Formats the Activity message and creates a scope.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <returns>A disposable scope object representing the lifetime of the logger scope.</returns>
    public static IDisposable? BeginActivityScope(this ILogger logger, LogActivityType type, JsonNode context) => _activityScope2(logger, type, context.ToJsonString());

    public static IDisposable? BeginActivityScope(this ILogger logger, LogActivityType type, string? context = null) => (context is null) ? _activityScope1(logger, type) :
        _activityScope2(logger, type, context);

    #endregion

    #region Critical DbfileValidationError (0x0001)

    /// <summary>
    /// Numerical event code for database file validation error.
    /// </summary>
    public const int EVENT_ID_DbfileValidationError = 0x0001;

    public const string TEMPLATE_DbfileValidationError_Unexpected = "Unexpected error validating DB file path \"{DbFile}\".";

    public const string TEMPLATE_DbfileValidationError_Invalid = "DB file path is invalid: \"{DbFile}\"";

    public const string TEMPLATE_DbfileValidationError_TooLong = "DB file path is too long: {DbFile}";

    /// <summary>
    /// Event ID for database file validation error.
    /// </summary>
    public static readonly EventId DbfileValidationError = new(EVENT_ID_DbfileValidationError, nameof(DbfileValidationError));

    private static readonly Action<ILogger, string, Exception?> _dbfileValidationError = LoggerMessage.Define<string>(LogLevel.Critical, DbfileValidationError, TEMPLATE_DbfileValidationError_Unexpected);

    private static readonly Action<ILogger, string, Exception?> _dbfilePathInvalid = LoggerMessage.Define<string>(LogLevel.Critical, DbfileValidationError, TEMPLATE_DbfileValidationError_Invalid);

    private static readonly Action<ILogger, string, Exception?> _dbfilePathTooLong = LoggerMessage.Define<string>(LogLevel.Critical, DbfileValidationError, TEMPLATE_DbfileValidationError_TooLong);

    /// <summary>
    /// Logs a database validation error event (DbfileValidationError) with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbfilePathInvalid(this ILogger? logger, string dbFile, NotSupportedException error)
    {
        if (logger is null)
            Serilog.Log.Logger.Fatal(error, TEMPLATE_DbfileValidationError_Invalid, dbFile);
        else
            _dbfilePathInvalid(logger, dbFile, error);
    }

    /// <summary>
    /// Logs a database validation error event (DbfileValidationError) with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbfilePathTooLong(this ILogger? logger, string dbFile, PathTooLongException error)
    {
        if (logger is null)
            Serilog.Log.Logger.Fatal(error, TEMPLATE_DbfileValidationError_TooLong, dbFile);
        else
            _dbfilePathTooLong(logger, dbFile, error);
    }

    /// <summary>
    /// Logs a database validation error event (DbfileValidationError) with event code 0x0001.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbfileValidationError(this ILogger? logger, string dbFile, Exception error)
    {
        if (logger is null)
            Serilog.Log.Logger.Fatal(error, TEMPLATE_DbFileAccessError, dbFile);
        else
            _dbfileValidationError(logger, dbFile, error);
    }

    #endregion

    #region Critical DbFileDirectoryNotFound Error (0x0002)

    /// <summary>
    /// Numerical event code for DbFileDirectoryNotFound.
    /// </summary>
    public const int EVENT_ID_DbFileDirectoryNotFound = 0x0002;

    /// <summary>
    /// Event ID for DbFileDirectoryNotFound.
    /// </summary>
    public static readonly EventId DbFileDirectoryNotFound = new(EVENT_ID_DbFileDirectoryNotFound, nameof(DbFileDirectoryNotFound));

    private static readonly Action<ILogger, string, Exception?> _dbFileDirectoryNotFound = LoggerMessage.Define<string>(LogLevel.Critical, DbFileDirectoryNotFound,
        "Parent directory for database file {Path} does not exist.");

    /// <summary>
    /// Logs an DbFileDirectoryNotFound event with event code 0x0002.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFilePath">The database file path.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogDbFileDirectoryNotFound(this ILogger logger, string dbFilePath, Exception? error = null) => _dbFileDirectoryNotFound(logger, dbFilePath, error);

    #endregion

    #region Critical DbFileAccessError (0x0003)

    /// <summary>
    /// Numerical event code for database file access error.
    /// </summary>
    public const int EVENT_ID_DbFileAccessError = 0x0003;

    public const string TEMPLATE_DbFileAccessError = "Unable to create DB file \"{Dbfile}\".";

    /// <summary>
    /// Event ID for database file access error.
    /// </summary>
    public static readonly EventId DbFileAccessError = new(EVENT_ID_DbFileAccessError, nameof(DbFileAccessError));

    private static readonly Action<ILogger, string, Exception?> _dbFileAccessError = LoggerMessage.Define<string>(LogLevel.Critical, DbFileAccessError, TEMPLATE_DbFileAccessError);

    /// <summary>
    /// Logs a database file access error event (DbfileAccessError) with event code 0x0003.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The path of the database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbFileAccessError(this ILogger? logger, string dbFile, Exception error)
    {
        if (logger is null)
            Serilog.Log.Logger.Fatal(error, TEMPLATE_DbFileAccessError, dbFile);
        else
            _dbFileAccessError(logger, dbFile, error);
    }

    #endregion

    #region Critical DbInitializationFailure (0x0004)

    /// <summary>
    /// Numerical event code for database initialization error.
    /// </summary>
    public const int EVENT_ID_DbInitializationFailure = 0x0004;

    /// <summary>
    /// Event ID for database initialization error.
    /// </summary>
    public static readonly EventId DbInitializationFailure = new(EVENT_ID_DbInitializationFailure, nameof(DbInitializationFailure));

    private static readonly Action<ILogger, int, CommandType?, string?, Type, string, Exception> _dbInitializationFailure1 = LoggerMessage.Define<int, CommandType?, string?, Type, string>(LogLevel.Critical, DbInitializationFailure,
        "Error code {ErrorCode} while executing DB initialization {CommandType} query {CommandText} for {EntityType} in {DbPath}.");

    private static readonly Action<ILogger, string, Type, string, Exception> _dbInitializationFailure2 = LoggerMessage.Define<string, Type, string>(LogLevel.Critical, DbInitializationFailure,
        "Unexpected error while executing DB initialization query {QueryString} for {EntityType} in {DbPath}.");

    private static readonly Action<ILogger, string?, int?, string?, CommandType?, string?, Exception> _dbInitializationFailure3 = LoggerMessage.Define<string?, int?, string?, CommandType?, string?>(LogLevel.Critical, DbInitializationFailure,
        "Error code {ErrorCode} ({SqlState}) while executing DB initialization {CommandType} query {CommandText} on {ConnectionString}.");

    private static readonly Action<ILogger, Type?, int?, string?, CommandType?, string?, string?, Exception> _dbInitializationFailure4 = LoggerMessage.Define<Type?, int?, string?, CommandType?, string?, string?>(LogLevel.Critical, DbInitializationFailure,
        "Error code {ErrorCode} ({SqlState}) while executing DB initialization {CommandType} query {CommandText} for {EntityType} in {DbPath}.");

    private static readonly Action<ILogger, string, Exception> _dbInitializationFailure5 = LoggerMessage.Define<string>(LogLevel.Critical, DbInitializationFailure,
        "Unexpected error while executing DB initialization for {DbPath}.");

    private static readonly Action<ILogger, string, Exception> _dbInitializationFailure6 = LoggerMessage.Define<string>(LogLevel.Critical, DbInitializationFailure,
        "Unexpected error while executing DB initialization for {ConnectionString}.");

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public static void LogDbInitializationFailure<T>(this ILogger logger, FileInfo dbFile, DbException error) =>
        _dbInitializationFailure1(logger, error.ErrorCode, error.BatchCommand?.CommandType, error.BatchCommand?.CommandText, typeof(T), dbFile.FullName, error);

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="querystring">The query string that failed.</param>
    /// <param name="type">The DB entity object type.</param>
    /// <param name="dbFile">The database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    /// <typeparam name="T">The entity type.</typeparam>
    public static void LogDbInitializationFailure<T>(this ILogger logger, string querystring, FileInfo dbFile, Exception error) =>
        _dbInitializationFailure2(logger, querystring, typeof(T), dbFile.FullName, error);

    public static void LogDbInitializationFailure(this ILogger logger, string connectionString, int? errorCode, string? sqlState, CommandType? commandType, string? commandText, Exception error)
    {
        _dbInitializationFailure3(logger, connectionString, errorCode, sqlState, commandType, commandText, error);
    }

    public static void LogDbInitializationFailure(this ILogger logger, Type? entityType, int? errorCode, string? sqlState, CommandType? commandType, string? commandText, string? dbPath, Exception error)
    {
        _dbInitializationFailure4(logger, entityType, errorCode, sqlState, commandType, commandText, dbPath, error);
    }

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="dbFile">The database file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbInitializationFailure(this ILogger logger, FileInfo dbFile, Exception error) => _dbInitializationFailure5(logger, dbFile.FullName, error);

    /// <summary>
    /// Logs a database initialization error event (DbInitializationFailure) with event code 0x0004.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogDbInitializationFailure(this ILogger logger, string connectionString, Exception error) => _dbInitializationFailure6(logger, connectionString, error);

    #endregion

    #region Critical CriticalSettingValueNotProvided Error (0x0005)

    /// <summary>
    /// Numerical event code for CriticalSettingValueNotProvided.
    /// </summary>
    public const int EVENT_ID_CriticalSettingValueNotProvided = 0x0005;

    /// <summary>
    /// Event ID for CriticalSettingValueNotProvided.
    /// </summary>
    public static readonly EventId CriticalSettingValueNotProvided = new(EVENT_ID_CriticalSettingValueNotProvided, nameof(CriticalSettingValueNotProvided));

    private static readonly Action<ILogger, string, Exception?> _criticalSettingValueNotProvided1 = LoggerMessage.Define<string>(LogLevel.Critical, CriticalSettingValueNotProvided,
        "The {SettingName} setting is empty or was not provided.");

    private static readonly Action<ILogger, string, char, Exception?> _criticalSettingValueNotProvided2 = LoggerMessage.Define<string, char>(LogLevel.Critical, CriticalSettingValueNotProvided,
        "The {SettingName} setting ({CmdLineSwitch}) is empty or was not not provided.");

    /// <summary>
    /// Logs an CriticalSettingValueNotProvided event with event code 0x0005.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="settingName">The name of the setting.</param>
    /// <param name="cmdLineSwitch">The command-line switch</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogCriticalSettingValueNotProvided(this ILogger logger, string settingName, char? cmdLineSwitch = null)
    {
        if (cmdLineSwitch.HasValue)
            _criticalSettingValueNotProvided2(logger, settingName, cmdLineSwitch.Value, null);
        else
            _criticalSettingValueNotProvided1(logger, settingName, null);
    }

    #endregion

    #region RenderMode Trace (0x0006)

    /// <summary>
    /// Numerical event code for RenderMode.
    /// </summary>
    public const int EVENT_ID_RenderMode = 0x0006;

    /// <summary>
    /// Event ID for RenderMode.
    /// </summary>
    public static readonly EventId RenderMode = new(EVENT_ID_RenderMode, nameof(RenderMode));

    private static readonly Action<ILogger, string, char, string, Exception?> _renderMode1 = LoggerMessage.Define<string, char, string>(LogLevel.Debug, RenderMode,
        "Setting {Setting} (-{Switch}) is {Value}.");

    private static readonly Action<ILogger, string, char, string, Exception?> _renderMode2 = LoggerMessage.Define<string, char, string>(LogLevel.Debug, RenderMode,
        "Setting {Setting} (-{Switch}) defaulted to {Value}.");

    /// <summary>
    /// Logs an RenderMode event with event code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="isScoped">Indicates whether the mode is for scoped scripts.</param>
    public static void LogRenderModeSettingValue(this ILogger logger, bool isScoped) => throw new NotImplementedException(); // _renderMode1(logger, nameof(AppSettings.Mode), SHORTHAND_m, isScoped ? MODE_SCOPED : MODE_GLOBAL, null);

    /// <summary>
    /// Logs an RenderMode event with event code 0x0006.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="isScoped">Indicates whether the default mode is for scoped scripts.</param>
    public static void LogDefaultRenderMode(this ILogger logger, bool isScoped) => throw new NotImplementedException(); //  _renderMode2(logger, nameof(AppSettings.Mode), SHORTHAND_m, isScoped ? MODE_SCOPED : MODE_GLOBAL, null);

    #endregion

    #region UsingOutputFile Trace (0x0007)

    /// <summary>
    /// Numerical event code for UsingOutputFile.
    /// </summary>
    public const int EVENT_ID_UsingOutputFile = 0x0007;

    /// <summary>
    /// Event ID for UsingOutputFile.
    /// </summary>
    public static readonly EventId UsingOutputFile = new(EVENT_ID_UsingOutputFile, nameof(UsingOutputFile));

    private static readonly Action<ILogger, string, bool, Exception?> _usingOutputFile = LoggerMessage.Define<string, bool>(LogLevel.Debug, UsingOutputFile,
        "Writing output to {$Path} (Overwrite is {OverWrite})");

    /// <summary>
    /// Logs an UsingOutputFile event with event code 0x0007.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The output file path.</param>
    /// <param name="overWrite">The value of the <see cref="AppSettings.ForceOverwrite"/> setting.</param>
    public static void LogUsingOutputFile(this ILogger logger, string path, bool overWrite) => _usingOutputFile(logger, path, overWrite, null);

    #endregion

    #region Critical InvalidRemoteInstanceUrl Error (0x0008)

    /// <summary>
    /// Numerical event code for invalid remote URL.
    /// </summary>
    public const int EVENT_ID_InvalidRemoteInstanceUrl = 0x0008;

    /// <summary>
    /// Event ID for invalid remote URL.
    /// </summary>
    public static readonly EventId InvalidRemoteInstanceUrl = new(EVENT_ID_InvalidRemoteInstanceUrl, nameof(InvalidRemoteInstanceUrl));

    private static readonly Action<ILogger, string, Exception?> _invalidRemoteInstanceUrl1 = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRemoteInstanceUrl,
        $"The {nameof(AppSettings.RemoteURL)} setting ({SHORTHAND_r}) contains an invalid URL: {{URI}} does not use the http or https scheme.");

    private static readonly Action<ILogger, string, Exception?> _invalidRemoteInstanceUrl2 = LoggerMessage.Define<string>(LogLevel.Critical, InvalidRemoteInstanceUrl,
        $"The {nameof(AppSettings.RemoteURL)} setting ({SHORTHAND_r}) contains an invalid URL: {{URI}} is not an absolute URI.");

    /// <summary>
    /// Logs an invalid remote URL event (InvalidRemoteInstanceUrl) with event code 0x0008.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The invalid URI.</param>
    public static void LogInvalidRemoteInstanceUrl(this ILogger logger, Uri uri)
    {
        if (uri.IsAbsoluteUri)
            _invalidRemoteInstanceUrl1(logger, uri.OriginalString, null);
        else
            _invalidRemoteInstanceUrl2(logger, uri.OriginalString, null);
    }

    #endregion

    #region NoTableNamesSpecified Warning (0x0009)

    /// <summary>
    /// Numerical event code for no table names provided.
    /// </summary>
    public const int EVENT_ID_NoTableNamesSpecified = 0x0009;

    /// <summary>
    /// Event ID for no table names provided.
    /// </summary>
    public static readonly EventId NoTableNamesSpecified = new(EVENT_ID_NoTableNamesSpecified, nameof(NoTableNamesSpecified));

    private static readonly Action<ILogger, Exception?> _noTableNamesSpecified = LoggerMessage.Define(LogLevel.Warning, NoTableNamesSpecified,
        "No table names were specified; nothing to do.");

    /// <summary>
    /// Logs a no table names provided event (NoTableNamesSpecified) with event code 0x0009.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="exception">The optional exception that caused the event.</param>
    public static void LogNoTableNamesSpecified(this ILogger logger) => _noTableNamesSpecified(logger, null);

    #endregion

    #region JsonFileAccess Error (0x000a)

    /// <summary>
    /// Numerical event code for JSON file access error.
    /// </summary>
    public const int EVENT_ID_JsonFileAccessError = 0x000a;

    public const string TEMPLATE_JsonFileAccessError = "Unable to read from JSON file \"{Name}\".";

    /// <summary>
    /// Event ID for JSON file access error.
    /// </summary>
    public static readonly EventId JsonFileAccessError = new(EVENT_ID_JsonFileAccessError, nameof(JsonFileAccessError));

    private static readonly Action<ILogger, string, Exception?> _jsonFileAccessError = LoggerMessage.Define<string>(LogLevel.Error, JsonFileAccessError, TEMPLATE_JsonFileAccessError);

    /// <summary>
    /// Logs a JSON file access error event (JsonFileAccessError) with event code 0x000a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="name">The name of the JSON file.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogJsonFileAccessError(this ILogger? logger, string name, Exception error)
    {
        if (logger is null)
            Serilog.Log.Logger.Error(error, TEMPLATE_JsonFileAccessError, name);
        else
            _jsonFileAccessError(logger, name, error);
    }

    #endregion

    #region Critical OutputFileAlreadyExists Error (0x000b)

    /// <summary>
    /// Numerical event code for an already-existing output file.
    /// </summary>
    public const int EVENT_ID_OutputFileAlreadyExists = 0x000b;

    /// <summary>
    /// Event ID for an already-existing output file.
    /// </summary>
    public static readonly EventId OutputFileAlreadyExists = new(EVENT_ID_OutputFileAlreadyExists, nameof(OutputFileAlreadyExists));

    private static readonly Action<ILogger, string, Exception?> _outputFileAlreadyExists = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAlreadyExists,
        "File {Path}");

    /// <summary>
    /// Logs an already-existing output file event (OutputFileAlreadyExists) with event code 0x000b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAlreadyExists(this ILogger logger, string path) => _outputFileAlreadyExists(logger, path, null);

    #endregion

    #region Critical OutputFileAccess Error (0x000c)

    /// <summary>
    /// Numerical event code for output file access error.
    /// </summary>
    public const int EVENT_ID_OutputFileAccessError = 0x000c;

    /// <summary>
    /// Event ID for output file access error.
    /// </summary>
    public static readonly EventId OutputFileAccessError = new(EVENT_ID_OutputFileAccessError, nameof(OutputFileAccessError));

    private static readonly Action<ILogger, string, Exception?> _outputFileAccessError1 = LoggerMessage.Define<string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}.");

    private static readonly Action<ILogger, string, string, Exception?> _outputFileAccessError2 = LoggerMessage.Define<string, string>(LogLevel.Critical, OutputFileAccessError,
        "Error accessing output file {Path}: {Reason}");

    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, Exception? error = null) => _outputFileAccessError1(logger, path, error);

    /// <summary>
    /// Logs an output file access error event (OutputFileAccessError) with event code 0x000c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="path">The path of the output file.</param>
    /// <param name="reason">The message describing the error.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogOutputFileAccessError(this ILogger logger, string path, string reason, Exception? error = null) => _outputFileAccessError2(logger, path, reason, error);

    #endregion

    #region HttpRequestFailed Error (0x000d)

    /// <summary>
    /// Numerical event code for HTTP request failure.
    /// </summary>
    public const int EVENT_ID_HttpRequestFailed = 0x000d;

    /// <summary>
    /// Event ID for HTTP request failure.
    /// </summary>
    public static readonly EventId HttpRequestFailed = new(EVENT_ID_HttpRequestFailed, nameof(HttpRequestFailed));

    private static readonly Action<ILogger, Uri, Exception?> _httpRequestFailed1 = LoggerMessage.Define<Uri>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed.");

    private static readonly Action<ILogger, Uri, string, Exception?> _httpRequestFailed2 = LoggerMessage.Define<Uri, string>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed: {Message}");

    private static readonly Action<ILogger, Uri, int, string, Exception?> _httpRequestFailed3 = LoggerMessage.Define<Uri, int, string>(LogLevel.Error, HttpRequestFailed,
        "Remote request {URI} failed with error code {ErrorCode} ({Description}).");
    private static readonly Action<ILogger, Uri, int, string, string, Exception?> _httpRequestFailed4 = LoggerMessage.Define<Uri, int, string, string>(LogLevel.Error, HttpRequestFailed,
    "Remote request {URI} failed with error code {ErrorCode} ({Description}): {Message}");

    /// <summary>
    /// Logs an HTTP request failure event (HttpRequestFailed) with event code 0x000d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI that failed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogHttpRequestFailed(this ILogger logger, Uri uri, Exception? error = null)
    {
        if (error is null)
            _httpRequestFailed1(logger, uri, error);
        else if (error is HttpRequestException exception && exception.StatusCode.HasValue)
        {
            if (string.IsNullOrWhiteSpace(error.Message))
                _httpRequestFailed3(logger, uri, (int)exception.StatusCode.Value, exception.StatusCode.Value.ToDisplayName(), error);
            else
                _httpRequestFailed4(logger, uri, (int)exception.StatusCode.Value, exception.StatusCode.Value.ToDisplayName(), error.Message, error);
        }
        else if (string.IsNullOrWhiteSpace(error.Message))
            _httpRequestFailed1(logger, uri, error);
        else
            _httpRequestFailed2(logger, uri, error.Message, error);
    }

    #endregion

    #region GetResponseContentFailed Error (0x000e)

    /// <summary>
    /// Numerical event code for HTTP response parsing error.
    /// </summary>
    public const int EVENT_ID_GetResponseContentFailed = 0x000e;

    /// <summary>
    /// Event ID for HTTP response parsing error.
    /// </summary>
    public static readonly EventId GetResponseContentFailed = new(EVENT_ID_GetResponseContentFailed, nameof(GetResponseContentFailed));

    private static readonly Action<ILogger, Uri, Exception?> _getResponseContentFailed = LoggerMessage.Define<Uri>(LogLevel.Error, GetResponseContentFailed,
        "Failed to get text-based content from remote URI {URI}");

    /// <summary>
    /// Logs a HTTP response parsing error event (GetResponseContentFailed) with event code 0x000e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request uri.</param>
    /// <param name="error">The optional exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogGetResponseContentFailed(this ILogger logger, Uri uri, Exception? error = null) => _getResponseContentFailed(logger, uri, error);

    #endregion

    #region JsonCouldNotBeParsed Error (0x000f)

    /// <summary>
    /// Numerical event code for JSON parsing error.
    /// </summary>
    public const int EVENT_ID_JsonCouldNotBeParsed = 0x000f;

    /// <summary>
    /// Event ID for JSON parsing error.
    /// </summary>
    public static readonly EventId JsonCouldNotBeParsed = new(EVENT_ID_JsonCouldNotBeParsed, nameof(JsonCouldNotBeParsed));

    private static readonly Action<ILogger, Uri, string, Exception?> _jsonCouldNotBeParsed = LoggerMessage.Define<Uri, string>(LogLevel.Error, JsonCouldNotBeParsed,
        "Unable to parse response from {URI}; Content: {Content}");

    /// <summary>
    /// Logs a JSON parsing error event (JsonCouldNotBeParsed) with event code 0x000f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="content">The content that could not be parsed.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogJsonCouldNotBeParsed(this ILogger logger, Uri uri, string content, Exception? error) => _jsonCouldNotBeParsed(logger, uri, content, error);

    #endregion

    #region InvalidHttpResponse Error (0x0010)

    /// <summary>
    /// Numerical event code for invalid HTTP response.
    /// </summary>
    public const int EVENT_ID_InvalidHttpResponse = 0x0010;

    /// <summary>
    /// Event ID for invalid HTTP response.
    /// </summary>
    public static readonly EventId InvalidHttpResponse = new(EVENT_ID_InvalidHttpResponse, nameof(InvalidHttpResponse));

    private static readonly Action<ILogger, Uri, string, Exception?> _invalidHttpResponse1 = LoggerMessage.Define<Uri, string>(LogLevel.Error, InvalidHttpResponse,
        "Response from {URI} did not match the expected format. Content: {Content}");

    private static readonly Action<ILogger, Uri, string, string, string?, Exception?> _invalidHttpResponse2 = LoggerMessage.Define<Uri, string, string, string?>(LogLevel.Error, InvalidHttpResponse,
        "Response from {URI} did not match the expected content type. Expected: {Expected}; Actual: {Actual}; Content: {Content}");

    /// <summary>
    /// Logs an invalid HTTP response event (InvalidHttpResponse) with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The response text.</param>
    /// <param name="error">The exception that caused the event</param>
    public static void LogInvalidHttpResponse(this ILogger logger, Uri uri, JsonNode? response, Exception? error = null) => _invalidHttpResponse1(logger, uri, (response is null) ? "null" : response.ToJsonString(), error);

    /// <summary>
    /// Logs an invalid HTTP response event (InvalidHttpResponse) with event code 0x0010.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The response text.</param>
    /// <param name="error">The exception that caused the event</param>
    public static void LogInvalidHttpResponse(this ILogger logger, Uri uri, string actualContentType, string? response, Exception? error = null) => _invalidHttpResponse2(logger, uri, System.Net.Mime.MediaTypeNames.Application.Json, actualContentType, response, error);

    #endregion

    #region ValidatingEntity Trace (0x0011)

    /// <summary>
    /// Numerical event code for ValidatingEntity.
    /// </summary>
    public const int EVENT_ID_ValidatingEntity = 0x0011;

    /// <summary>
    /// Event ID for ValidatingEntity.
    /// </summary>
    public static readonly EventId ValidatingEntity = new(EVENT_ID_ValidatingEntity, nameof(ValidatingEntity));

    private static readonly Action<ILogger, EntityState, string, object, Exception?> _validatingEntity = LoggerMessage.Define<EntityState, string, object>(LogLevel.Debug, ValidatingEntity,
        "Validating {State} {Name} {Entity}");

    /// <summary>
    /// Logs a ValidatingEntity event with event code 0x0011.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state while being validated.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidatingEntity(this ILogger logger, EntityState state, IEntityType metadata, object entity)
    {
        string displayName = metadata.DisplayName()?.Trim()!;
        if (string.IsNullOrEmpty(displayName) && string.IsNullOrEmpty(displayName = metadata.Name?.Trim()!))
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(displayName = t.FullName!))
                displayName = t.Name;
        }
        _validatingEntity(logger, state, displayName, entity, null);
    }

    #endregion

    #region EntityValidationFailure Error (0x0012)

    /// <summary>
    /// Numerical event code for EntityValidationFailure.
    /// </summary>
    public const int EVENT_ID_EntityValidationFailure = 0x0012;

    /// <summary>
    /// Event ID for EntityValidationFailure.
    /// </summary>
    public static readonly EventId EntityValidationFailure = new(EVENT_ID_EntityValidationFailure, nameof(EntityValidationFailure));

    private static readonly Action<ILogger, string, string, object, ValidationException> _entityValidationFailure1 =
        LoggerMessage.Define<string, string, object>(LogLevel.Error, EntityValidationFailure, "Error Validating {Name} ({ValidationMessage}) {Entity}");

    private static readonly Action<ILogger, string, string, string, object, ValidationException> _entityValidationFailure2 =
        LoggerMessage.Define<string, string, string, object>(LogLevel.Error, EntityValidationFailure, "Error Validating {Name} [{Properties}] ({ValidationMessage}) {Entity}");

    /// <summary>
    /// Logs an EntityValidationFailure event with event code 0x0012.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogEntityValidationFailure(this ILogger logger, IEntityType metadata, object entity, ValidationException error)
    {
        IEnumerable<string> memberNames = error.ValidationResult.MemberNames.Where(n => !string.IsNullOrWhiteSpace(n));
        string name = metadata.DisplayName()?.Trim()!;
        if (name.Length == 0)
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(name = t.FullName!))
                name = t.Name;
        }
        string message = error.ValidationResult.ErrorMessage?.Trim()!;
        if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(message = error.Message?.Trim()!))
            _entityValidationFailure1(logger, name, memberNames.Any() ? $"Validation error on {string.Join(", ", memberNames)}" : "Validation Failure", entity, error);
        else if (memberNames.Any())
            _entityValidationFailure1(logger, name, message, entity, error);
        else
            _entityValidationFailure2(logger, name, string.Join(", ", memberNames), message, entity, error);
    }

    #endregion

    #region ValidationCompleted Trace (0x0013)

    /// <summary>
    /// Numerical event code for ValidationCompleted.
    /// </summary>
    public const int EVENT_ID_ValidationCompleted = 0x0013;

    /// <summary>
    /// Event ID for ValidationCompleted.
    /// </summary>
    public static readonly EventId ValidationCompleted = new(EVENT_ID_ValidationCompleted, nameof(ValidationCompleted));

    private static readonly Action<ILogger, EntityState, string, object, Exception?> _validationCompleted =
        LoggerMessage.Define<EntityState, string, object>(LogLevel.Debug, ValidationCompleted, "Validation for {State} {Name} {Entity}");

    /// <summary>
    /// Logs a ValidationCompleted event with event code 0x0013.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="state">The entity state during validation.</param>
    /// <param name="metadata">The entity metadata.</param>
    /// <param name="entity">The entity object.</param>
    public static void LogValidationCompleted(this ILogger logger, EntityState state, IEntityType metadata, object entity)
    {
        string name = metadata.DisplayName()?.Trim()!;
        if (name.Length == 0)
        {
            Type t = entity.GetType();
            if (string.IsNullOrWhiteSpace(name = t.FullName!))
                name = t.Name;
        }
        _validationCompleted(logger, state, name, entity, null);
    }

    #endregion

    #region DbSaveChangesCompleted Trace (0x0014)

    /// <summary>
    /// Numerical event code for DbSaveChangesCompleted.
    /// </summary>
    public const int EVENT_ID_DbSaveChangesCompleted = 0x0014;

    /// <summary>
    /// Event ID for DbSaveChangesCompleted.
    /// </summary>
    public static readonly EventId DbSaveChangesCompleted = new(EVENT_ID_DbSaveChangesCompleted, nameof(DbSaveChangesCompleted));

    private static readonly Action<ILogger, string, int, Exception?> _dbSaveChangesCompleted = LoggerMessage.Define<string, int>(LogLevel.Debug, DbSaveChangesCompleted,
        "Message {MethodSignature} {ReturnValue}");

    /// <summary>
    /// Logs an DbSaveChangesCompleted event with event code 0x0014.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="methodSignature">The first event parameter.</param>
    /// <param name="returnValue">The second event parameter.</param>
    public static void LogDbSaveChangesCompleted(this ILogger logger, bool isAsync, bool? acceptAllChangesOnSuccess, int returnValue) => _dbSaveChangesCompleted(logger, isAsync ?
        (acceptAllChangesOnSuccess.HasValue ? $"SaveChangesAsync({acceptAllChangesOnSuccess.Value})" : "SaveChangesAsync()") :
        acceptAllChangesOnSuccess.HasValue ? $"SaveChanges({acceptAllChangesOnSuccess.Value})" : "SaveChanges()", returnValue, null);

    #endregion

    #region InvalidResponseType Error (0x0015)

    /// <summary>
    /// Numerical event code for InvalidResponseType.
    /// </summary>
    public const int EVENT_ID_InvalidResponseType = 0x0015;

    /// <summary>
    /// Event ID for InvalidResponseType.
    /// </summary>
    public static readonly EventId InvalidResponseType = new(EVENT_ID_InvalidResponseType, nameof(InvalidResponseType));

    private static readonly Action<ILogger, Uri, Exception?> _invalidResponseType1 = LoggerMessage.Define<Uri>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned null.");

    private static readonly Action<ILogger, Uri, string, string, Exception?> _invalidResponseType2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, InvalidResponseType,
        "Response from {URI} retuned unexpected type {Type}. Actual Result: {Result}");

    /// <summary>
    /// Logs an InvalidResponseType event with event code 0x0015.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogInvalidResponseType(this ILogger logger, Uri uri, JsonNode? response, Exception? error = null)
    {
        if (response is null)
            _invalidResponseType1(logger, uri, null);
        else
            _invalidResponseType2(logger, uri, response.GetType().Name, response.ToJsonString(), error);
    }

    #endregion

    #region ResponseResultPropertyNotFound Error (0x0016)

    /// <summary>
    /// Numerical event code for ResponseResultPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ResponseResultPropertyNotFound = 0x0016;

    /// <summary>
    /// Event ID for ResponseResultPropertyNotFound.
    /// </summary>
    public static readonly EventId ResponseResultPropertyNotFound = new(EVENT_ID_ResponseResultPropertyNotFound, nameof(ResponseResultPropertyNotFound));

    private static readonly Action<ILogger, Uri, string, Exception?> _responseResultPropertyNotFound = LoggerMessage.Define<Uri, string>(LogLevel.Error, ResponseResultPropertyNotFound,
        $"Response from  {{URI}} did not contain a property named \"{JSON_KEY_RESULT}\". Actual Response: {{Response}}");

    /// <summary>
    /// Logs an ResponseResultPropertyNotFound event with event code 0x0016.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogResponseResultPropertyNotFound(this ILogger logger, Uri uri, JsonObject response, Exception? error = null) => _responseResultPropertyNotFound(logger, uri, response.ToJsonString(), error);

    #endregion

    #region NoResultsFromQuery Error (0x0017)

    /// <summary>
    /// Numerical event code for NoResultsFromQuery.
    /// </summary>
    public const int EVENT_ID_NoResultsFromQuery = 0x0017;

    /// <summary>
    /// Event ID for NoResultsFromQuery.
    /// </summary>
    public static readonly EventId NoResultsFromQuery = new(EVENT_ID_NoResultsFromQuery, nameof(NoResultsFromQuery));

    private static readonly Action<ILogger, Uri, string, Exception?> _noResultsFromQuery = LoggerMessage.Define<Uri, string>(LogLevel.Error, NoResultsFromQuery,
        "Response from {URI} returned no results. Actual response: {Response}");

    /// <summary>
    /// Logs an NoResultsFromQuery event with event code 0x0017.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="response">The actual response.</param>
    public static void LogNoResultsFromQuery(this ILogger logger, Uri uri, JsonObject response) => _noResultsFromQuery(logger, uri, response.ToJsonString(), null);

    #endregion

    #region MultipleResponseItems Warning (0x0018)

    /// <summary>
    /// Numerical event code for MultipleResponseItems.
    /// </summary>
    public const int EVENT_ID_MultipleResponseItems = 0x0018;

    /// <summary>
    /// Event ID for MultipleResponseItems.
    /// </summary>
    public static readonly EventId MultipleResponseItems = new(EVENT_ID_MultipleResponseItems, nameof(MultipleResponseItems));

    private static readonly Action<ILogger, Uri, int, string, Exception?> _multipleResponseItems = LoggerMessage.Define<Uri, int, string>(LogLevel.Warning, MultipleResponseItems,
        "Response from  returned  additional values. Actual response: {URI} {AdditionalCount} {Response}");

    /// <summary>
    /// Logs an MultipleResponseItems event with event code 0x0018.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="additionalCount">The number of additional elements.</param>
    /// <param name="response">The actual response.</param>
    public static void LogMultipleResponseItems(this ILogger logger, Uri uri, int additionalCount, JsonObject response) => _multipleResponseItems(logger, uri, additionalCount, response.ToJsonString(), null);

    #endregion

    #region InvalidResultElementType Error (0x0019)

    /// <summary>
    /// Numerical event code for InvalidResultElementType.
    /// </summary>
    public const int EVENT_ID_InvalidResultElementType = 0x0019;

    /// <summary>
    /// Event ID for InvalidResultElementType.
    /// </summary>
    public static readonly EventId InvalidResultElementType = new(EVENT_ID_InvalidResultElementType, nameof(InvalidResultElementType));

    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _invalidResultElementType = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, InvalidResultElementType,
        "Response from {URI} had an unexpected type {Type} at index {Index}. Actual element: {JSON}");

    /// <summary>
    /// Logs an InvalidResultElementType event with event code 0x0019.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="type">The unexpected type.</param>
    /// <param name="index">The element index.</param>
    /// <param name="response">The actual element.</param>
    /// <param name="error">The exception that caused the event.</param>
    public static void LogInvalidResultElementType(this ILogger logger, Uri uri, JsonNode? element, int index, Exception? error = null)
    {
        if (element is null)
            _invalidResultElementType(logger, uri, "null", index, "", null);
        else
            _invalidResultElementType(logger, uri, element.GetType().Name, index, element.ToJsonString(), error);
    }

    #endregion

    #region ExpectedPropertyNotFound Error (0x001a)

    /// <summary>
    /// Numerical event code for ExpectedPropertyNotFound.
    /// </summary>
    public const int EVENT_ID_ExpectedPropertyNotFound = 0x001a;

    /// <summary>
    /// Event ID for ExpectedPropertyNotFound.
    /// </summary>
    public static readonly EventId ExpectedPropertyNotFound = new(EVENT_ID_ExpectedPropertyNotFound, nameof(ExpectedPropertyNotFound));

    private static readonly Action<ILogger, Uri, string, int, string, Exception?> _expectedPropertyNotFound1 = LoggerMessage.Define<Uri, string, int, string>(LogLevel.Error, ExpectedPropertyNotFound,
        "Response from {URI} is missing property {PropertyName} at index {Index}. Actual response: {Response}");

    private static readonly Action<ILogger, Uri, string, string, Exception?> _expectedPropertyNotFound2 = LoggerMessage.Define<Uri, string, string>(LogLevel.Error, ExpectedPropertyNotFound,
        "Response from {URI} is missing property {PropertyName}. Actual response: {Response}");

    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="index">The index of the result item.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFound(this ILogger logger, Uri uri, string propertyname, int index, JsonObject element, Exception? error = null) =>
        _expectedPropertyNotFound1(logger, uri, propertyname, index, element.ToJsonString(), error);

    /// <summary>
    /// Logs an ExpectedPropertyNotFound event with event code 0x001a.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The request URI.</param>
    /// <param name="propertyname">The name of the missing field.</param>
    /// <param name="response">The actual response.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogExpectedPropertyNotFound(this ILogger logger, Uri uri, string propertyname, JsonObject element, Exception? error = null) =>
        _expectedPropertyNotFound2(logger, uri, propertyname, element.ToJsonString(), error);

    #endregion

    #region APIRequestStart Trace (0x001b)

    /// <summary>
    /// Numerical event code for APIRequestStart.
    /// </summary>
    public const int EVENT_ID_APIRequestStart = 0x001b;

    /// <summary>
    /// Event ID for APIRequestStart.
    /// </summary>
    public static readonly EventId APIRequestStart = new(EVENT_ID_APIRequestStart, nameof(APIRequestStart));

    private static readonly Action<ILogger, Uri, Exception?> _apiRequestStart = LoggerMessage.Define<Uri>(LogLevel.Debug, APIRequestStart,
        "Sending API request to {URI}");

    /// <summary>
    /// Logs an APIRequestStart event with event code 0x001b.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="uri">The API requst URL.</param>
    public static void LogAPIRequestStart(this ILogger logger, Uri uri) => _apiRequestStart(logger, uri, null);

    #endregion

    #region APIRequestCompleted Trace (0x001c)

    /// <summary>
    /// Numerical event code for APIRequestCompleted.
    /// </summary>
    public const int EVENT_ID_APIRequestCompleted = 0x001c;

    /// <summary>
    /// Event ID for APIRequestCompleted.
    /// </summary>
    public static readonly EventId APIRequestCompleted = new(EVENT_ID_APIRequestCompleted, nameof(APIRequestCompleted));

    private static readonly Action<ILogger, Uri, string, Exception?> _apirequestCompleted = LoggerMessage.Define<Uri, string>(LogLevel.Debug, APIRequestCompleted,
        "API request  returned {URL} {Result}");

    /// <summary>
    /// Logs an APIRequestCompleted event with event code 0x001c.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="url">The request URL.</param>
    /// <param name="result">The parsed API result.</param>
    public static void LogAPIRequestCompleted(this ILogger logger, Uri url, JsonNode? result) => _apirequestCompleted(logger, url, (result is null) ? "null" : result.ToJsonString(), null);

    #endregion

    #region GettingTableByNameFromRemote Trace (0x001d)

    /// <summary>
    /// Numerical event code for GettingTableByNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTableByNameFromRemote = 0x001d;

    /// <summary>
    /// Event ID for GettingTableByNameFromRemote.
    /// </summary>
    public static readonly EventId GettingTableByNameFromRemote = new(EVENT_ID_GettingTableByNameFromRemote, nameof(GettingTableByNameFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingTableByNameFromRemote = LoggerMessage.Define<string>(LogLevel.Debug, GettingTableByNameFromRemote,
        "Getting table by name {Name} from remote instance.");

    /// <summary>
    /// Logs an GettingTableByNameFromRemote event with event code 0x001d.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="name">The name of the table being looked up.</param>
    public static void LogGettingTableByNameFromRemote(this ILogger logger, string name) => _gettingTableByNameFromRemote(logger, name, null);

    #endregion

    #region GettingTableBySysIdFromRemote Trace (0x001e)

    /// <summary>
    /// Numerical event code for GettingTableBySysIdFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTableBySysIdFromRemot = 0x001e;

    /// <summary>
    /// Event ID for GettingTableBySysIdFromRemote.
    /// </summary>
    public static readonly EventId GettingTableBySysIdFromRemote = new(EVENT_ID_GettingTableBySysIdFromRemot, nameof(GettingTableBySysIdFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingTableBySysIdFromRemote = LoggerMessage.Define<string>(LogLevel.Debug, GettingTableBySysIdFromRemote,
        "Getting table by Sys ID {SysID} from remote instance.");

    /// <summary>
    /// Logs an GettingTableBySysIdFromRemote event with event code 0x001e.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="sysID">The Sys ID of the table to look up.</param>
    public static void LogGettingTableBySysIdFromRemote(this ILogger logger, string sysID) => _gettingTableBySysIdFromRemote(logger, sysID, null);

    #endregion

    #region GettingElementsByTableNameFromRemote Trace (0x001f)

    /// <summary>
    /// Numerical event code for GettingElementsByTableNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingElementsByTableNameFromRemote = 0x001f;

    /// <summary>
    /// Event ID for GettingElementsByTableNameFromRemote.
    /// </summary>
    public static readonly EventId GettingElementsByTableNameFromRemote = new(EVENT_ID_GettingElementsByTableNameFromRemote, nameof(GettingElementsByTableNameFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingElementsByTableNameFromRemote = LoggerMessage.Define<string>(LogLevel.Debug, GettingElementsByTableNameFromRemote,
        "Getting elements from remote instance with table name {TableName}.");

    /// <summary>
    /// Logs an GettingElementsByTableNameFromRemote event with event code 0x001f.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogGettingElementsByTableNameFromRemote(this ILogger logger, string tableName) => _gettingElementsByTableNameFromRemote(logger, tableName, null);

    #endregion

    #region GettingScopeByIdentifierFromRemote Trace (0x0020)

    /// <summary>
    /// Numerical event code for GettingScopeByIdentifierFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingScopeByIdentifierFromRemote = 0x0020;

    /// <summary>
    /// Event ID for GettingScopeByIdentifierFromRemote.
    /// </summary>
    public static readonly EventId GettingScopeByIdentifierFromRemote = new(EVENT_ID_GettingScopeByIdentifierFromRemote, nameof(GettingScopeByIdentifierFromRemote));

    private static readonly Action<ILogger, string, Exception?> _gettingScopeByIdentifierFromRemote = LoggerMessage.Define<string>(LogLevel.Debug, GettingScopeByIdentifierFromRemote,
        "Getting scope by unique identifier {Identifer} from remote instance.");

    /// <summary>
    /// Logs an GettingScopeByIdentifierFromRemote event with event code 0x0020.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="identifer">The unique identifier of the sys_scope.</param>
    public static void LogGettingScopeByIdentifierFromRemote(this ILogger logger, string identifer) => _gettingScopeByIdentifierFromRemote(logger, identifer, null);

    #endregion

    #region GettingTypeByNameFromRemote Trace (0x0021)

    /// <summary>
    /// Numerical event code for GettingTypeByNameFromRemote.
    /// </summary>
    public const int EVENT_ID_GettingTypeByNameFromRemoteTrace = 0x0021;

    /// <summary>
    /// Event ID for GettingTypeByNameFromRemote.
    /// </summary>
    public static readonly EventId GettingTypeByNameFromRemoteTrace = new(EVENT_ID_GettingTypeByNameFromRemoteTrace, nameof(GettingTypeByNameFromRemoteTrace));

    private static readonly Action<ILogger, string, Exception?> _gettingTypeByNameFromRemoteTrace = LoggerMessage.Define<string>(LogLevel.Debug, GettingTypeByNameFromRemoteTrace,
        "Getting type by name {TypeName} from remote instance.");

    /// <summary>
    /// Logs an GettingTypeByNameFromRemote event with event code 0x0021.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="typeName">The name of the sys_glide_object.</param>
    public static void LogGettingTypeByNameFromRemoteTrace(this ILogger logger, string typeName) => _gettingTypeByNameFromRemoteTrace(logger, typeName, null);

    #endregion

    #region AddingTableToDb Trace (0x0022)

    /// <summary>
    /// Numerical event code for AddingTableToDb.
    /// </summary>
    public const int EVENT_ID_AddingTableToDb = 0x0022;

    /// <summary>
    /// Event ID for AddingTableToDb.
    /// </summary>
    public static readonly EventId AddingTableToDb = new(EVENT_ID_AddingTableToDb, nameof(AddingTableToDb));

    private static readonly Action<ILogger, string, Exception?> _AddingTableToDb = LoggerMessage.Define<string>(LogLevel.Debug, AddingTableToDb,
        "Adding table {TableName} to database.");

    /// <summary>
    /// Logs an AddingTableToDb event with event code 0x0022.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table being added.</param>
    public static void LogAddingTableToDb(this ILogger logger, string tableName) => _AddingTableToDb(logger, tableName, null);

    #endregion

    #region AddingElementsToDatabase Trace (0x0023)

    /// <summary>
    /// Numerical event code for AddingElementsToDatabase.
    /// </summary>
    public const int EVENT_ID_AddingElementsToDatabase = 0x0023;

    /// <summary>
    /// Event ID for AddingElementsToDatabase.
    /// </summary>
    public static readonly EventId AddingElementsToDatabase = new(EVENT_ID_AddingElementsToDatabase, nameof(AddingElementsToDatabase));

    private static readonly Action<ILogger, string, Exception?> _AddingElementsToDatabase = LoggerMessage.Define<string>(LogLevel.Debug, AddingElementsToDatabase,
        "Adding elements for table {TableName} to database.");

    /// <summary>
    /// Logs an AddingElementsToDatabase event with event code 0x0023.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogAddingElementsToDatabase(this ILogger logger, string tableName) => _AddingElementsToDatabase(logger, tableName, null);

    #endregion

    #region NewTableSaveComplete Trace (0x0024)

    /// <summary>
    /// Numerical event code for NewTableSaveComplete.
    /// </summary>
    public const int EVENT_ID_NewTableSaveCompleted = 0x0024;

    /// <summary>
    /// Event ID for NewTableSaveComplete.
    /// </summary>
    public static readonly EventId NewTableSaveCompleted = new(EVENT_ID_NewTableSaveCompleted, nameof(NewTableSaveCompleted));

    private static readonly Action<ILogger, string, Exception?> _newTableSaveCompleted = LoggerMessage.Define<string>(LogLevel.Debug, NewTableSaveCompleted,
        "Table named {TableName} and related entites saved to database.");

    /// <summary>
    /// Logs an NewTableSaveComplete event with event code 0x0024.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="tableName">The name of the table.</param>
    public static void LogNewTableSaveCompleted(this ILogger logger, string tableName) => _newTableSaveCompleted(logger, tableName, null);

    #endregion

    #region InvalidJsClassMapping Error (0x0025)

    /// <summary>
    /// Numerical event code for Invalid JS Class Mapping error.
    /// </summary>
    public const int EVENT_ID_InvalidJsClassMapping = 0x0025;

    public const string TEMPLATE_InvalidJsClassMapping = "Invalid Class Mapping JSON object in \"{FileName}\", index {Index}.";

    /// <summary>
    /// Event ID for Invalid JS Class Mapping error.
    /// </summary>
    public static readonly EventId InvalidJsClassMapping = new(EVENT_ID_InvalidJsClassMapping, nameof(InvalidJsClassMapping));

    private static readonly Action<ILogger, string, int, Exception?> _invalidJsClassMapping = LoggerMessage.Define<string, int>(LogLevel.Error, InvalidJsClassMapping, TEMPLATE_InvalidJsClassMapping);

    /// <summary>
    /// Logs a Invalid JS Class Mapping error event (InvalidJsClassMapping) with event code 0x0025.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The name of the JSON file.</param>
    /// <param name="index">The index of the invalid element.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogInvalidJsClassMapping(this ILogger? logger, string fileName, int index, Exception? error = null)
    {
        if (logger is null)
            Serilog.Log.Logger.Error(error, TEMPLATE_InvalidJsClassMapping, fileName, index);
        else
            _invalidJsClassMapping(logger, fileName, index, error);
    }

    #endregion

    #region DuplicateJsClassMapping Warning (0x0026)

    /// <summary>
    /// Numerical event code for Duplicate JS Class Mapping warning.
    /// </summary>
    public const int EVENT_ID_DuplicateJsClassMapping = 0x0026;

    public const string TEMPLATE_DuplicateJsClassMapping = $"Ignoring Class Mapping JSON object with duplicate {nameof(JsClassMapping.JsClass)} \"{{JsClass}}\" in \"{{FileName}}\", index {{Index}}.";

    /// <summary>
    /// Event ID for Duplicate JS Class Mapping warning.
    /// </summary>
    public static readonly EventId DuplicateJsClassMapping = new(EVENT_ID_DuplicateJsClassMapping, nameof(DuplicateJsClassMapping));

    private static readonly Action<ILogger, string, string, int, Exception?> _duplicateJsClassMapping = LoggerMessage.Define<string, string, int>(LogLevel.Warning, DuplicateJsClassMapping, TEMPLATE_DuplicateJsClassMapping);

    /// <summary>
    /// Logs a Duplicate JS Class Mapping warning event (DuplicateJsClassMapping) with event code 0x0026.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="jsClass">The value of the duplicate key.</param>
    /// <param name="fileName">The name of the JSON file.</param>
    /// <param name="index">The index of the duplicate element.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogDuplicateJsClassMapping(this ILogger? logger, string jsClass, string fileName, int index, Exception? error = null)
    {
        if (logger is null)
            Serilog.Log.Logger.Warning(error, TEMPLATE_DuplicateJsClassMapping, jsClass, index);
        else
            _duplicateJsClassMapping(logger, jsClass, fileName, index, error);
    }

    #endregion

    #region InvalidGlideTypeJson Error (0x0027)

    /// <summary>
    /// Numerical event code for Invalid Glide Type error.
    /// </summary>
    public const int EVENT_ID_InvalidGlideTypeJson = 0x0027;

    public const string TEMPLATE_InvalidGlideTypeJson = "Invalid Glide Type JSON object in \"{FileName}\", index {Index}.";

    /// <summary>
    /// Event ID for Invalid Glide Type error.
    /// </summary>
    public static readonly EventId InvalidGlideTypeJson = new(EVENT_ID_InvalidGlideTypeJson, nameof(InvalidGlideTypeJson));

    private static readonly Action<ILogger, string, int, Exception?> _invalidGlideTypeJson = LoggerMessage.Define<string, int>(LogLevel.Error, InvalidGlideTypeJson, TEMPLATE_InvalidGlideTypeJson);

    /// <summary>
    /// Logs a Invalid Glide Type error event (InvalidGlideTypeJson) with event code 0x0027.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="fileName">The name of the JSON file.</param>
    /// <param name="index">The index of the invalid element.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogInvalidGlideTypeJson(this ILogger? logger, string fileName, int index, Exception? error = null)
    {
        if (logger is null)
            Serilog.Log.Logger.Error(error, TEMPLATE_InvalidGlideTypeJson, fileName, index);
        else
            _invalidGlideTypeJson(logger, fileName, index, error);
    }

    #endregion

    #region DuplicateGlideTypeJson Warning (0x0028)

    /// <summary>
    /// Numerical event code for Duplicate Glide Type warning.
    /// </summary>
    public const int EVENT_ID_DuplicateGlideTypeJson = 0x0028;

    public const string TEMPLATE_DuplicateGlideTypeJson = $"Ignoring Glide Type JSON object with duplicate {nameof(KnownGlideType.Name)} \"{{Name}}\" in \"{{FileName}}\", index {{Index}}.";

    /// <summary>
    /// Event ID for Duplicate Glide Type warning.
    /// </summary>
    public static readonly EventId DuplicateGlideTypeJson = new(EVENT_ID_DuplicateGlideTypeJson, nameof(DuplicateGlideTypeJson));

    private static readonly Action<ILogger, string, string, int, Exception?> _duplicateGlideTypeJson = LoggerMessage.Define<string, string, int>(LogLevel.Warning, DuplicateGlideTypeJson, TEMPLATE_DuplicateGlideTypeJson);

    /// <summary>
    /// Logs a Duplicate Glide Type warning event (DuplicateGlideTypeJson) with event code 0x0028.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="name">The name of the duplicate type.</param>
    /// <param name="fileName">The name of the JSON file.</param>
    /// <param name="index">The index of the duplicate element.</param>
    /// <param name="error">The optional exception that caused the event.</param>
    public static void LogDuplicateGlideTypeJson(this ILogger? logger, string name, string fileName, int index, Exception? error = null)
    {
        if (logger is null)
            Serilog.Log.Logger.Warning(error, TEMPLATE_DuplicateGlideTypeJson, name, index);
        else
            _duplicateGlideTypeJson(logger, name, fileName, index, error);
    }

    #endregion

    #region Critical UnexpecteException Error (0x00ff)

    /// <summary>
    /// Numerical event code for UnexpecteException.
    /// </summary>
    public const int EVENT_ID_UnexpectedException = 0x00ff;

    /// <summary>
    /// Event ID for UnexpecteException.
    /// </summary>
    public static readonly EventId UnexpectedException = new(EVENT_ID_UnexpectedException, nameof(UnexpectedException));

    private static readonly Action<ILogger, Exception?> _unexpectedException1 = LoggerMessage.Define(LogLevel.Critical, UnexpectedException,
        "An unexpected exception has occurred.");


    private static readonly Action<ILogger, string, Exception?> _unexpectedException2 = LoggerMessage.Define<string>(LogLevel.Critical, UnexpectedException,
        "An unexpected exception has occurred - Activity: {Activity}");

    /// <summary>
    /// Logs an UnexpecteException event with event code 0x00ff.
    /// </summary>
    /// <param name="logger">The current logger.</param>
    /// <param name="error">The exception that caused the event or <see langword="null" /> for no exception.</param>
    public static void LogUnexpectedException(this ILogger logger, Exception error, string? activity = null)
    {
        if (string.IsNullOrWhiteSpace(activity) && string.IsNullOrWhiteSpace(activity = error?.Message))
            _unexpectedException1(logger, error);
        else
            _unexpectedException2(logger, activity, error);
    }

    #endregion
}
using System.Text.Json.Nodes;
using SnTsTypeGenerator.Services;
using static SnTsTypeGenerator.Services.SnApiConstants;

namespace SnTsTypeGenerator.Models.TableAPI;

/// <summary>
/// Deserialized record reference.
/// </summary>
/// <param name="Value">The value of the <c>value</c> property.</param>
/// <param name="DisplayValue">The value of the <c>display_value</c> property.</param>
public record RecordRef(string Value, string DisplayValue)
{
    internal static RecordRef? DeserializeProperty(JsonObject obj, string propertyName) => (obj.TryGetProperty(propertyName, out JsonObject? p) && p.TryGetPropertyAsNonEmpty(JSON_KEY_VALUE, out string? value)) ?
        new(Value: value, DisplayValue: p.CoercePropertyAsNonEmpty(JSON_KEY_DISPLAY_VALUE, value)) : null;
}

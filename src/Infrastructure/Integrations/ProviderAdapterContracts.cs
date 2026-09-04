using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;

internal static class ProviderJson
{
    public static JsonElement Object(string json, string name)
    {
        try
        {
            JsonElement root = JsonSerializer.Deserialize<JsonElement>(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            if (root.ValueKind != JsonValueKind.Object) throw new JsonException();
            return root;
        }
        catch (JsonException exception)
        {
            throw new ProviderAdapterException("configuration_invalid", $"{name} must be a JSON object.", false, innerException: exception);
        }
    }

    public static string? String(this JsonElement value, string name) =>
        value.TryGetProperty(name, out JsonElement property) && property.ValueKind == JsonValueKind.String ? property.GetString() : null;

    public static int Int32(this JsonElement value, string name, int fallback) =>
        value.TryGetProperty(name, out JsonElement property) && property.TryGetInt32(out int result) ? result : fallback;

    public static decimal? Decimal(this JsonElement value, string name) =>
        value.TryGetProperty(name, out JsonElement property) && property.TryGetDecimal(out decimal result) ? result : null;

    public static bool Boolean(this JsonElement value, string name, bool fallback) =>
        value.TryGetProperty(name, out JsonElement property) && property.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? property.GetBoolean() : fallback;
}

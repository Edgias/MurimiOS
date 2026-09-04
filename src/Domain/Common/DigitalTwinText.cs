using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.Common;

internal static class DigitalTwinText
{
    public static string Required(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"Text must contain 1-{maximum} characters.");
        return result;
    }
    public static string? Optional(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length > maximum) throw new ArgumentException($"Text cannot exceed {maximum} characters.");
        return result.Length == 0 ? null : result;
    }
    public static string JsonObject(string? value, int maximum)
    {
        string result = Required(value, maximum);
        try
        {
            using JsonDocument document = JsonDocument.Parse(result);
            if (document.RootElement.ValueKind != JsonValueKind.Object) throw new ArgumentException("Details must be a JSON object.");
            return document.RootElement.GetRawText();
        }
        catch (JsonException exception) { throw new ArgumentException("Details must be valid JSON.", exception); }
    }
}

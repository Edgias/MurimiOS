using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.EpcisExchange;

public sealed class EpcisCapturedEvent : TenantEntity
{
    private EpcisCapturedEvent() { }

    public EpcisCapturedEvent(Guid id, Guid exchangeId, Guid farmId, string eventId,
        string eventType, DateTimeOffset eventTime, DateTimeOffset? recordTime, string? businessStep,
        string? disposition, string objectIdentifiersJson, string sha256, string payloadJson) : base(id)
    {
        if (exchangeId == Guid.Empty || farmId == Guid.Empty) throw new ArgumentException("Exchange and farm identifiers are required.");
        ExchangeId = exchangeId;
        FarmId = farmId;
        EventId = Absolute(eventId, nameof(eventId));
        EventType = Text(eventType, 40);
        EventTime = eventTime.ToUniversalTime();
        RecordTime = recordTime?.ToUniversalTime();
        BusinessStep = Optional(businessStep, 500);
        Disposition = Optional(disposition, 500);
        ObjectIdentifiersJson = EpcisExchange.Json(objectIdentifiersJson, JsonValueKind.Array, 500_000);
        Sha256 = Hash(sha256);
        PayloadJson = EpcisExchange.Json(payloadJson, JsonValueKind.Object, 2_000_000);
    }

    public Guid ExchangeId { get; private set; }
    public Guid FarmId { get; private set; }
    public string EventId { get; private set; } = "";
    public string EventType { get; private set; } = "";
    public DateTimeOffset EventTime { get; private set; }
    public DateTimeOffset? RecordTime { get; private set; }
    public string? BusinessStep { get; private set; }
    public string? Disposition { get; private set; }
    public string ObjectIdentifiersJson { get; private set; } = "[]";
    public string Sha256 { get; private set; } = "";
    public string PayloadJson { get; private set; } = "{}";

    private static string Text(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"Text must contain 1-{maximum} characters.");
        return result;
    }
    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Text cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
    private static string Absolute(string value, string parameter)
    {
        string result = Text(value, 500);
        if (!System.Uri.TryCreate(result, UriKind.Absolute, out _)) throw new ArgumentException("An absolute URI is required.", parameter);
        return result;
    }
    private static string Hash(string value)
    {
        string result = value.Trim().ToLowerInvariant();
        if (result.Length != 64 || result.Any(x => !System.Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 digest is required.");
        return result;
    }
}

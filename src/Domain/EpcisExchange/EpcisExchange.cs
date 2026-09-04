using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.EpcisExchange;

public sealed class EpcisExchange : TenantEntity
{
    private EpcisExchange() { }

    public EpcisExchange(Guid id, Guid farmId, EpcisExchangeDirection direction, string documentType,
        string documentId, string schemaVersion, string cbvVersion, string sha256, int eventCount,
        DateTimeOffset? firstEventAt, DateTimeOffset? lastEventAt, string? sender, string? receiver,
        string payloadJson, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (farmId == Guid.Empty || !Enum.IsDefined(direction) || eventCount < 0
            || firstEventAt > lastEventAt) throw new ArgumentException("EPCIS exchange identity, direction, count, or event range is invalid.");
        FarmId = farmId;
        Direction = direction;
        DocumentType = Required(documentType, 40);
        DocumentId = Uri(documentId, nameof(documentId));
        SchemaVersion = Required(schemaVersion, 20);
        CbvVersion = Required(cbvVersion, 20);
        Sha256 = Hash(sha256);
        EventCount = eventCount;
        FirstEventAt = firstEventAt?.ToUniversalTime();
        LastEventAt = lastEventAt?.ToUniversalTime();
        Sender = Optional(sender, 500);
        Receiver = Optional(receiver, 500);
        PayloadJson = Json(payloadJson, JsonValueKind.Object, 10_000_000);
        ExchangedAt = createdAt.ToUniversalTime();
        ExchangedBy = Required(createdBy, 240);
    }

    public Guid FarmId { get; private set; }
    public EpcisExchangeDirection Direction { get; private set; }
    public string DocumentType { get; private set; } = "";
    public string DocumentId { get; private set; } = "";
    public string SchemaVersion { get; private set; } = "";
    public string CbvVersion { get; private set; } = "";
    public string Sha256 { get; private set; } = "";
    public int EventCount { get; private set; }
    public DateTimeOffset? FirstEventAt { get; private set; }
    public DateTimeOffset? LastEventAt { get; private set; }
    public string? Sender { get; private set; }
    public string? Receiver { get; private set; }
    public string PayloadJson { get; private set; } = "{}";
    public DateTimeOffset ExchangedAt { get; private set; }
    public string ExchangedBy { get; private set; } = "";

    internal static string Json(string value, JsonValueKind expected, int maximum)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > maximum) throw new ArgumentException("EPCIS JSON is empty or too large.");
        using JsonDocument document = JsonDocument.Parse(value);
        if (document.RootElement.ValueKind != expected) throw new ArgumentException($"EPCIS JSON must be a {expected}.");
        return document.RootElement.GetRawText();
    }

    private static string Required(string? value, int maximum)
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

    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? "";
        if (result.Length != 64 || result.Any(x => !System.Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 digest is required.");
        return result;
    }

    private static string Uri(string value, string parameter)
    {
        string result = Required(value, 500);
        if (!System.Uri.TryCreate(result, UriKind.Absolute, out _)) throw new ArgumentException("An absolute URI is required.", parameter);
        return result;
    }
}

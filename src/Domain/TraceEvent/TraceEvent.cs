using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;

namespace EdgIQ.MurimiOS.Domain.TraceEvent;

public sealed class TraceEvent : TenantEntity
{
    private TraceEvent() { }

    public TraceEvent(Guid id, Guid farmId, TraceEventType eventType, string businessStep,
        string disposition, DateTimeOffset occurredAt, string locationType, Guid? locationId,
        string sourceType, Guid sourceId, string? transactionReference, string? payloadJson = null) : base(id)
    {
        if (farmId == Guid.Empty || sourceId == Guid.Empty)
            throw new ArgumentException("Farm and immutable source identifiers are required.");
        FarmId = farmId;
        EventType = eventType;
        BusinessStep = Text(businessStep, nameof(businessStep), 120);
        Disposition = Text(disposition, nameof(disposition), 120);
        OccurredAt = occurredAt;
        RecordedAt = DateTimeOffset.UtcNow;
        LocationType = Text(locationType, nameof(locationType), 80);
        LocationId = locationId;
        SourceType = Text(sourceType, nameof(sourceType), 120);
        SourceId = sourceId;
        TransactionReference = Optional(transactionReference, 160);
        PayloadJson = string.IsNullOrWhiteSpace(payloadJson) ? "{}" : payloadJson;
    }

    public Guid FarmId { get; private set; }
    public TraceEventType EventType { get; private set; }
    public string BusinessStep { get; private set; } = string.Empty;
    public string Disposition { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string LocationType { get; private set; } = string.Empty;
    public Guid? LocationId { get; private set; }
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }
    public string? TransactionReference { get; private set; }
    public string PayloadJson { get; private set; } = "{}";

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }

    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

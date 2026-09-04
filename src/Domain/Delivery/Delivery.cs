using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public enum DeliveryStatus { Dispatched = 1, Received = 2, Cancelled = 3 }

public sealed class Delivery : TenantEntity
{
    private Delivery() { }
    public Delivery(Guid id, Guid farmId, string deliveryNumber, string recipient, string destination,
        DateTimeOffset dispatchedAt, string? transactionReference) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        FarmId = farmId;
        DeliveryNumber = Text(deliveryNumber, nameof(deliveryNumber), 120);
        Recipient = Text(recipient, nameof(recipient), 240);
        Destination = Text(destination, nameof(destination), 240);
        DispatchedAt = dispatchedAt;
        TransactionReference = Optional(transactionReference, 160);
        Status = DeliveryStatus.Dispatched;
    }

    public Guid FarmId { get; private set; }
    public string DeliveryNumber { get; private set; } = string.Empty;
    public string Recipient { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public DateTimeOffset DispatchedAt { get; private set; }
    public DateTimeOffset? ReceivedAt { get; private set; }
    public string? TransactionReference { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public void MarkReceived(DateTimeOffset receivedAt)
    {
        if (Status != DeliveryStatus.Dispatched) throw new InvalidOperationException("Only a dispatched delivery can be received.");
        if (receivedAt < DispatchedAt) throw new ArgumentOutOfRangeException(nameof(receivedAt));
        ReceivedAt = receivedAt;
        Status = DeliveryStatus.Received;
    }

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

public sealed class DeliveryLine : TenantEntity
{
    private DeliveryLine() { }
    public DeliveryLine(Guid id, Guid deliveryId, TraceObjectType sourceType, Guid sourceId, decimal quantity, string unit) : base(id)
    {
        if (deliveryId == Guid.Empty || sourceId == Guid.Empty) throw new ArgumentException("Delivery and source identifiers are required.");
        if (sourceType is not (TraceObjectType.HarvestLot or TraceObjectType.StorageLot)) throw new ArgumentException("Only crop lots can be delivered.");
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        DeliveryId = deliveryId;
        SourceType = sourceType;
        SourceId = sourceId;
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
    }
    public Guid DeliveryId { get; private set; }
    public TraceObjectType SourceType { get; private set; }
    public Guid SourceId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

public sealed class LivestockFeedBatch : TenantEntity
{
    private LivestockFeedBatch() { }
    public LivestockFeedBatch(Guid id, Guid farmId, string batchCode, string feedName, decimal quantity,
        string unit, DateOnly receivedOn, DateOnly? expiresOn, string? supplier) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (expiresOn < receivedOn) throw new ArgumentException("Feed expiry cannot precede receipt.");
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume))
            throw new ArgumentException("Feed quantities must use mass or volume units.", nameof(unit));
        FarmId = farmId; BatchCode = Text(batchCode, nameof(batchCode), 120); FeedName = Text(feedName, nameof(feedName), 200);
        InitialQuantity = measured.CanonicalValue; QuantityOnHand = measured.CanonicalValue; Unit = measured.CanonicalUnit;
        EnteredInitialQuantity = measured.EnteredValue; EnteredUnit = measured.EnteredUnit; DisplayUnit = measured.DisplayUnit;
        ReceivedOn = receivedOn; ExpiresOn = expiresOn; Supplier = Optional(supplier, 200);
    }
    public Guid FarmId { get; private set; }
    public string BatchCode { get; private set; } = string.Empty;
    public string FeedName { get; private set; } = string.Empty;
    public decimal InitialQuantity { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredInitialQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;
    public DateOnly ReceivedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public string? Supplier { get; private set; }
    public void Issue(decimal canonicalQuantity) { if (canonicalQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(canonicalQuantity)); if (canonicalQuantity > QuantityOnHand) throw new InvalidOperationException("Insufficient feed stock."); QuantityOnHand -= canonicalQuantity; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

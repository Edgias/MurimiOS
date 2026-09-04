using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropFeedTransfer;

public sealed class CropFeedTransfer : TenantEntity
{
    private CropFeedTransfer() { }

    public CropFeedTransfer(Guid id, Guid farmId, CropFeedSourceType sourceType, Guid sourceId,
        Guid feedBatchId, Guid traceEventId, decimal quantity, string unit, DateTimeOffset transferredAt,
        string transferredBy, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || sourceId == Guid.Empty || feedBatchId == Guid.Empty || traceEventId == Guid.Empty)
            throw new ArgumentException("Farm, source, feed batch, and trace event identifiers are required.");
        if (!Enum.IsDefined(sourceType) || quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume))
            throw new ArgumentException("Crop-to-feed transfers must use mass or volume units.", nameof(unit));
        FarmId = farmId;
        SourceType = sourceType;
        SourceId = sourceId;
        FeedBatchId = feedBatchId;
        TraceEventId = traceEventId;
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        TransferredAt = transferredAt.ToUniversalTime();
        TransferredBy = Text(transferredBy, nameof(transferredBy), 240);
        Notes = Optional(notes, 2_000);
    }

    public Guid FarmId { get; private set; }
    public CropFeedSourceType SourceType { get; private set; }
    public Guid SourceId { get; private set; }
    public Guid FeedBatchId { get; private set; }
    public Guid TraceEventId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public DateTimeOffset TransferredAt { get; private set; }
    public string TransferredBy { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    private static string Text(string? value, string parameter, int max)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > max) throw new ArgumentException($"{parameter} is required and cannot exceed {max} characters.", parameter);
        return result;
    }
    private static string? Optional(string? value, int max)
    {
        string? result = value?.Trim();
        if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

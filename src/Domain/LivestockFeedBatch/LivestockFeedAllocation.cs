using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

public sealed class LivestockFeedAllocation : TenantEntity
{
    private LivestockFeedAllocation() { }
    public LivestockFeedAllocation(Guid id, Guid subjectId, Guid feedBatchId, decimal quantity, string unit,
        DateTimeOffset occurredAt, Guid? sourceCommandId, string? notes) : base(id)
    {
        if (subjectId == Guid.Empty || feedBatchId == Guid.Empty) throw new ArgumentException("Subject and feed batch are required.");
        if (sourceCommandId == Guid.Empty) throw new ArgumentException("A source command must be non-empty when supplied.", nameof(sourceCommandId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume))
            throw new ArgumentException("Feed allocations must use mass or volume units.", nameof(unit));
        SubjectId = subjectId; FeedBatchId = feedBatchId; Quantity = measured.CanonicalValue; Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue; EnteredUnit = measured.EnteredUnit; DisplayUnit = measured.DisplayUnit;
        OccurredAt = occurredAt; SourceCommandId = sourceCommandId; Notes = Optional(notes, 2_000);
    }
    public Guid SubjectId { get; private set; }
    public Guid FeedBatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public Guid? SourceCommandId { get; private set; }
    public string? Notes { get; private set; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

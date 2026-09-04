using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockRationPlan;

public sealed class LivestockRationLine : TenantEntity
{
    private LivestockRationLine() { }
    public LivestockRationLine(Guid id, Guid planId, Guid? feedBatchId, string feedName, decimal quantityPerHeadPerDay,
        string unit, decimal dryMatterPercent) : base(id)
    {
        if (planId == Guid.Empty || feedBatchId == Guid.Empty) throw new ArgumentException("A ration plan and valid optional feed batch are required.");
        if (quantityPerHeadPerDay <= 0 || dryMatterPercent is <= 0 or > 100) throw new ArgumentOutOfRangeException(nameof(quantityPerHeadPerDay));
        MeasuredQuantity captured = MeasurementUnits.Capture(quantityPerHeadPerDay, unit, unit);
        if (captured.Dimension != UnitDimension.Mass) throw new ArgumentException("Ration quantities must use mass units.");
        PlanId = planId; FeedBatchId = feedBatchId; FeedName = Text(feedName, 200); QuantityKgPerHeadPerDay = captured.CanonicalValue;
        EnteredQuantity = captured.EnteredValue; EnteredUnit = captured.EnteredUnit; DryMatterPercent = dryMatterPercent;
    }
    public Guid PlanId { get; private set; }
    public Guid? FeedBatchId { get; private set; }
    public string FeedName { get; private set; } = string.Empty;
    public decimal QuantityKgPerHeadPerDay { get; private set; }
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public decimal DryMatterPercent { get; private set; }
    private static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
}

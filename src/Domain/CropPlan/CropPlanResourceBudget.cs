using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropPlan;

public sealed class CropPlanResourceBudget : TenantEntity
{
    private CropPlanResourceBudget() { }
    public CropPlanResourceBudget(Guid id, Guid planId, Guid allocationId, Guid? plannedOperationId,
        PlannedResourceType resourceType, Guid? agriculturalInputId, string resourceName,
        decimal quantity, string unit, decimal unitCostPerCanonicalUnit, string currency) : base(id)
    {
        if (planId == Guid.Empty || allocationId == Guid.Empty || plannedOperationId == Guid.Empty || agriculturalInputId == Guid.Empty)
            throw new ArgumentException("Plan and allocation are required; optional identifiers must be non-empty.");
        if (!Enum.IsDefined(resourceType)) throw new ArgumentOutOfRangeException(nameof(resourceType));
        if (resourceType == PlannedResourceType.Input && agriculturalInputId is null)
            throw new ArgumentException("An input budget requires an agricultural input.", nameof(agriculturalInputId));
        if (resourceType != PlannedResourceType.Input && agriculturalInputId is not null)
            throw new ArgumentException("Only input budgets can reference an agricultural input.", nameof(agriculturalInputId));
        if (unitCostPerCanonicalUnit < 0) throw new ArgumentOutOfRangeException(nameof(unitCostPerCanonicalUnit));
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension == UnitDimension.Temperature || measured.CanonicalValue <= 0)
            throw new ArgumentException("A positive resource quantity is required.", nameof(quantity));
        PlanId = planId; AllocationId = allocationId; PlannedOperationId = plannedOperationId;
        ResourceType = resourceType; AgriculturalInputId = agriculturalInputId;
        ResourceName = Text(resourceName, nameof(resourceName), 160);
        Quantity = measured.CanonicalValue; Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue; EnteredUnit = measured.EnteredUnit;
        UnitCostPerCanonicalUnit = unitCostPerCanonicalUnit;
        Currency = new CurrencyCode(currency).Value;
    }
    public Guid PlanId { get; private set; }
    public Guid AllocationId { get; private set; }
    public Guid? PlannedOperationId { get; private set; }
    public PlannedResourceType ResourceType { get; private set; }
    public Guid? AgriculturalInputId { get; private set; }
    public string ResourceName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public decimal UnitCostPerCanonicalUnit { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal PlannedCost => decimal.Round(Quantity * UnitCostPerCanonicalUnit, 2, MidpointRounding.AwayFromZero);
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

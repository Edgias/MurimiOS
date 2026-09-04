using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropPlan;

public sealed class CropPlanAllocation : TenantEntity
{
    private CropPlanAllocation() { }

    public CropPlanAllocation(Guid id, Guid planId, Guid fieldId, Guid boundaryVersionId, Guid? cropZoneId,
        string crop, string? variety, decimal plannedArea, string areaUnit,
        decimal targetYield, string targetYieldUnit, decimal targetPricePerCanonicalUnit,
        string currency, DateOnly plannedSowingDate, DateOnly plannedHarvestDate) : base(id)
    {
        if (planId == Guid.Empty || fieldId == Guid.Empty || boundaryVersionId == Guid.Empty || cropZoneId == Guid.Empty)
            throw new ArgumentException("Plan, field, and boundary identifiers are required; an optional crop-zone id must be non-empty.");
        if (plannedHarvestDate < plannedSowingDate)
            throw new ArgumentException("Planned harvest cannot precede planned sowing.");
        if (targetPricePerCanonicalUnit < 0)
            throw new ArgumentOutOfRangeException(nameof(targetPricePerCanonicalUnit));
        MeasuredQuantity area = MeasurementUnits.Capture(plannedArea, areaUnit, areaUnit);
        if (area.Dimension != UnitDimension.Area || area.CanonicalValue <= 0)
            throw new ArgumentException("A positive planned area is required.", nameof(plannedArea));
        MeasuredQuantity yield = MeasurementUnits.Capture(targetYield, targetYieldUnit, targetYieldUnit);
        if (yield.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count) || yield.CanonicalValue <= 0)
            throw new ArgumentException("A positive mass, volume, or count yield target is required.", nameof(targetYield));
        PlanId = planId;
        FieldId = fieldId;
        BoundaryVersionId = boundaryVersionId;
        CropZoneId = cropZoneId;
        Crop = Text(crop, nameof(crop), 120);
        Variety = Optional(variety, 120);
        PlannedAreaHa = area.CanonicalValue;
        EnteredArea = area.EnteredValue;
        EnteredAreaUnit = area.EnteredUnit;
        TargetYield = yield.CanonicalValue;
        TargetYieldUnit = yield.CanonicalUnit;
        EnteredTargetYield = yield.EnteredValue;
        EnteredTargetYieldUnit = yield.EnteredUnit;
        TargetPricePerCanonicalUnit = targetPricePerCanonicalUnit;
        Currency = new CurrencyCode(currency).Value;
        PlannedSowingDate = plannedSowingDate;
        PlannedHarvestDate = plannedHarvestDate;
    }

    public Guid PlanId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public string Crop { get; private set; } = string.Empty;
    public string? Variety { get; private set; }
    public decimal PlannedAreaHa { get; private set; }
    public decimal EnteredArea { get; private set; }
    public string EnteredAreaUnit { get; private set; } = string.Empty;
    public decimal TargetYield { get; private set; }
    public string TargetYieldUnit { get; private set; } = string.Empty;
    public decimal EnteredTargetYield { get; private set; }
    public string EnteredTargetYieldUnit { get; private set; } = string.Empty;
    public decimal TargetPricePerCanonicalUnit { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateOnly PlannedSowingDate { get; private set; }
    public DateOnly PlannedHarvestDate { get; private set; }
    public decimal PlannedRevenue => decimal.Round(TargetYield * TargetPricePerCanonicalUnit, 2, MidpointRounding.AwayFromZero);

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

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;

public sealed class CropProductApplicationRule : TenantEntity
{
    private CropProductApplicationRule() { }
    public CropProductApplicationRule(Guid id, Guid inputId, string crop, string jurisdiction,
        decimal maximumRatePerHa, string rateUnit, int reEntryHours, int preHarvestIntervalDays,
        DateOnly effectiveFrom, DateOnly? effectiveTo, string authorizationReference) : base(id)
    {
        if (inputId == Guid.Empty) throw new ArgumentException("An agricultural input is required.", nameof(inputId));
        if (maximumRatePerHa <= 0 || reEntryHours < 0 || preHarvestIntervalDays < 0)
            throw new ArgumentException("Maximum rate must be positive and safety intervals cannot be negative.");
        if (effectiveTo < effectiveFrom) throw new ArgumentException("Rule expiry cannot precede its effective date.");
        MeasurementUnitDefinition rate = MeasurementUnits.Get(rateUnit);
        if (rate.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Application rate must use mass, volume, or count per hectare.", nameof(rateUnit));
        InputId = inputId; Crop = Text(crop, nameof(crop), 120); Jurisdiction = Text(jurisdiction, nameof(jurisdiction), 80).ToUpperInvariant();
        MaximumRatePerHa = MeasurementUnits.Convert(maximumRatePerHa, rate.Code, rate.CanonicalCode);
        RateUnit = rate.CanonicalCode; EnteredMaximumRatePerHa = maximumRatePerHa; EnteredRateUnit = rate.Code;
        ReEntryHours = reEntryHours; PreHarvestIntervalDays = preHarvestIntervalDays;
        EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo;
        AuthorizationReference = Text(authorizationReference, nameof(authorizationReference), 160);
    }
    public Guid InputId { get; private set; }
    public string Crop { get; private set; } = string.Empty;
    public string Jurisdiction { get; private set; } = string.Empty;
    public decimal MaximumRatePerHa { get; private set; }
    public string RateUnit { get; private set; } = string.Empty;
    public decimal EnteredMaximumRatePerHa { get; private set; }
    public string EnteredRateUnit { get; private set; } = string.Empty;
    public int ReEntryHours { get; private set; }
    public int PreHarvestIntervalDays { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string AuthorizationReference { get; private set; } = string.Empty;

    public void EnsurePermitted(CropPlanAllocation allocation, CropPlanResourceBudget budget, DateOnly applicationDate)
    {
        if (!Crop.Equals(allocation.Crop, StringComparison.OrdinalIgnoreCase)
            || applicationDate < EffectiveFrom || EffectiveTo is { } end && applicationDate > end)
            throw new InvalidOperationException("The product is not authorized for this crop and planned application date.");
        if (budget.AgriculturalInputId != InputId || !budget.Unit.Equals(RateUnit, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The planned product quantity does not match the authorization unit.");
        decimal rate = budget.Quantity / allocation.PlannedAreaHa;
        if (rate > MaximumRatePerHa)
            throw new InvalidOperationException($"Planned rate {rate:0.####} {RateUnit}/ha exceeds the authorized maximum {MaximumRatePerHa:0.####} {RateUnit}/ha.");
        if (applicationDate.AddDays(PreHarvestIntervalDays) > allocation.PlannedHarvestDate)
            throw new InvalidOperationException("The planned application violates the pre-harvest interval.");
    }

    public void EnsureReEntryInterval(PlannedCropOperation application,
        IEnumerable<PlannedCropOperation> allocationOperations)
    {
        if (ReEntryHours == 0) return;
        DateOnly earliestReEntryDate = application.PlannedStart.AddDays((ReEntryHours + 23) / 24);
        bool conflicts = allocationOperations.Any(other => other.Id != application.Id
            && (other.PlannedStart > application.PlannedStart
                || other.PlannedStart == application.PlannedStart && other.Sequence > application.Sequence)
            && other.PlannedStart < earliestReEntryDate);
        if (conflicts)
            throw new InvalidOperationException(
                $"Planned field work violates the {ReEntryHours}-hour re-entry interval; the earliest date-granular re-entry is {earliestReEntryDate:yyyy-MM-dd}.");
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

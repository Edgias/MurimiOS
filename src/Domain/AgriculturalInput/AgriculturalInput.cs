using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.AgriculturalInput;

public sealed class AgriculturalInput : TenantEntity
{
    private AgriculturalInput()
    {
    }

    public AgriculturalInput(Guid id, Guid farmId, string name, string category, string unit) : base(id)
    {
        if (farmId == Guid.Empty)
        {
            throw new ArgumentException("A farm is required.", nameof(farmId));
        }

        FarmId = farmId;
        Name = RequireText(name, nameof(name), 160);
        Category = RequireText(category, nameof(category), 80);
        MeasurementUnitDefinition measurementUnit = MeasurementUnits.Get(unit);
        if (measurementUnit.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Agricultural inputs must use a mass, volume, or count unit.", nameof(unit));
        Unit = measurementUnit.CanonicalCode;
        DisplayUnit = measurementUnit.Code;
    }

    public Guid FarmId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Category { get; private set; } = string.Empty;

    public string Unit { get; private set; } = string.Empty;

    public string DisplayUnit { get; private set; } = string.Empty;

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }
}

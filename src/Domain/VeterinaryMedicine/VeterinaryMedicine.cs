using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.VeterinaryMedicine;

public sealed class VeterinaryMedicine : TenantEntity
{
    private VeterinaryMedicine() { }

    public VeterinaryMedicine(
        Guid id,
        Guid farmId,
        string name,
        string activeIngredient,
        string unit,
        int meatWithdrawalDays,
        int milkWithdrawalHours) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (meatWithdrawalDays is < 0 or > 3650) throw new ArgumentOutOfRangeException(nameof(meatWithdrawalDays));
        if (milkWithdrawalHours is < 0 or > 87600) throw new ArgumentOutOfRangeException(nameof(milkWithdrawalHours));

        FarmId = farmId;
        Name = RequireText(name, nameof(name), 160);
        ActiveIngredient = RequireText(activeIngredient, nameof(activeIngredient), 160);
        MeasurementUnitDefinition measurementUnit = MeasurementUnits.Get(unit);
        if (measurementUnit.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Veterinary medicines must use a mass, volume, or count unit.", nameof(unit));
        Unit = measurementUnit.CanonicalCode;
        DisplayUnit = measurementUnit.Code;
        MeatWithdrawalDays = meatWithdrawalDays;
        MilkWithdrawalHours = milkWithdrawalHours;
    }

    public Guid FarmId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ActiveIngredient { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;
    public int MeatWithdrawalDays { get; private set; }
    public int MilkWithdrawalHours { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }
}

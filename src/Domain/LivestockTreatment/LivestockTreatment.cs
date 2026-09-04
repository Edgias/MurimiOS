using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockTreatment;

public sealed class LivestockTreatment : TenantEntity
{
    private LivestockTreatment() { }

    public LivestockTreatment(
        Guid id,
        Guid subjectId,
        Guid medicineStockLotId,
        decimal dose,
        string unit,
        string route,
        DateTimeOffset administeredAt,
        DateTimeOffset recordedAt,
        int meatWithdrawalDays,
        int milkWithdrawalHours,
        Guid sourceCommandId,
        string? notes) : base(id)
    {
        if (subjectId == Guid.Empty || medicineStockLotId == Guid.Empty || sourceCommandId == Guid.Empty)
            throw new ArgumentException("Subject, medicine stock lot, and source command are required.");
        if (dose <= 0) throw new ArgumentOutOfRangeException(nameof(dose));
        if (meatWithdrawalDays is < 0 or > 3650) throw new ArgumentOutOfRangeException(nameof(meatWithdrawalDays));
        if (milkWithdrawalHours is < 0 or > 87600) throw new ArgumentOutOfRangeException(nameof(milkWithdrawalHours));

        SubjectId = subjectId;
        MedicineStockLotId = medicineStockLotId;
        MeasuredQuantity measured = MeasurementUnits.Capture(dose, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Treatment doses must use a mass, volume, or count unit.", nameof(unit));
        Dose = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredDose = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
        Route = RequireText(route, nameof(route), 80);
        AdministeredAt = administeredAt;
        RecordedAt = recordedAt;
        MeatHoldUntil = meatWithdrawalDays == 0 ? null : administeredAt.AddDays(meatWithdrawalDays);
        MilkHoldUntil = milkWithdrawalHours == 0 ? null : administeredAt.AddHours(milkWithdrawalHours);
        SourceCommandId = sourceCommandId;
        Notes = OptionalText(notes, 2_000);
    }

    public Guid SubjectId { get; private set; }
    public Guid MedicineStockLotId { get; private set; }
    public decimal Dose { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredDose { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public DateTimeOffset AdministeredAt { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public DateTimeOffset? MeatHoldUntil { get; private set; }
    public DateTimeOffset? MilkHoldUntil { get; private set; }
    public Guid SourceCommandId { get; private set; }
    public string? Notes { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }

    private static string? OptionalText(string? value, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength) throw new ArgumentException($"Notes cannot exceed {maximumLength} characters.", nameof(value));
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

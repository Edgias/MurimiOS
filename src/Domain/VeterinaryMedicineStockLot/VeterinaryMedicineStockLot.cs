using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;

public sealed class VeterinaryMedicineStockLot : TenantEntity
{
    private VeterinaryMedicineStockLot() { }

    public VeterinaryMedicineStockLot(
        Guid id,
        Guid medicineId,
        string batchNumber,
        DateOnly receivedOn,
        DateOnly? expiresOn,
        decimal initialQuantity,
        string unit) : base(id)
    {
        if (medicineId == Guid.Empty) throw new ArgumentException("A medicine is required.", nameof(medicineId));
        if (initialQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(initialQuantity));
        if (expiresOn < receivedOn) throw new ArgumentException("Expiry cannot precede receipt.", nameof(expiresOn));

        MedicineId = medicineId;
        BatchNumber = RequireText(batchNumber, nameof(batchNumber), 120);
        ReceivedOn = receivedOn;
        ExpiresOn = expiresOn;
        MeasuredQuantity measured = MeasurementUnits.Capture(initialQuantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Medicine stock must use a mass, volume, or count unit.", nameof(unit));
        InitialQuantity = measured.CanonicalValue;
        QuantityOnHand = measured.CanonicalValue;
        EnteredInitialQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
    }

    public Guid MedicineId { get; private set; }
    public string BatchNumber { get; private set; } = string.Empty;
    public DateOnly ReceivedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public decimal InitialQuantity { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public decimal EnteredInitialQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;

    public void Issue(decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > QuantityOnHand) throw new InvalidOperationException("The treatment dose exceeds medicine stock on hand.");
        QuantityOnHand -= quantity;
    }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }
}

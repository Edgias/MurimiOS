using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.InputStockLot;

public sealed class InputStockLot : TenantEntity
{
    private InputStockLot()
    {
    }

    public InputStockLot(
        Guid id,
        Guid inputId,
        string lotCode,
        DateOnly receivedOn,
        DateOnly? expiresOn,
        decimal initialQuantity,
        string unit) : base(id)
    {
        if (inputId == Guid.Empty)
        {
            throw new ArgumentException("An agricultural input is required.", nameof(inputId));
        }

        string normalizedCode = lotCode?.Trim() ?? string.Empty;
        if (normalizedCode.Length is 0 or > 120)
        {
            throw new ArgumentException("A lot code between 1 and 120 characters is required.", nameof(lotCode));
        }

        if (initialQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Initial quantity must be positive.");
        }

        if (expiresOn < receivedOn)
        {
            throw new ArgumentException("Expiry cannot precede receipt.", nameof(expiresOn));
        }

        InputId = inputId;
        LotCode = normalizedCode;
        ReceivedOn = receivedOn;
        ExpiresOn = expiresOn;
        MeasuredQuantity measured = MeasurementUnits.Capture(initialQuantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Stock quantities must use a mass, volume, or count unit.", nameof(unit));
        InitialQuantity = measured.CanonicalValue;
        QuantityOnHand = measured.CanonicalValue;
        EnteredInitialQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
    }

    public Guid InputId { get; private set; }

    public string LotCode { get; private set; } = string.Empty;

    public DateOnly ReceivedOn { get; private set; }

    public DateOnly? ExpiresOn { get; private set; }

    public decimal InitialQuantity { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public decimal EnteredInitialQuantity { get; private set; }

    public string EnteredUnit { get; private set; } = string.Empty;

    public string DisplayUnit { get; private set; } = string.Empty;

    public void Issue(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Issue quantity must be positive.");
        }

        if (quantity > QuantityOnHand)
        {
            throw new InvalidOperationException("The issue quantity exceeds stock on hand.");
        }

        QuantityOnHand -= quantity;
    }
}

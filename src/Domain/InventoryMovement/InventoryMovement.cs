using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.InventoryMovement;

public enum InventoryMovementType
{
    Receipt = 1,
    Issue = 2,
    AdjustmentIncrease = 3,
    AdjustmentDecrease = 4
}

public sealed class InventoryMovement : TenantEntity
{
    private InventoryMovement()
    {
    }

    public InventoryMovement(
        Guid id,
        Guid stockLotId,
        InventoryMovementType type,
        decimal quantity,
        string unit,
        DateTimeOffset occurredAt,
        Guid? operationId,
        Guid? sourceCommandId,
        string? notes) : base(id)
    {
        if (stockLotId == Guid.Empty)
        {
            throw new ArgumentException("A stock lot is required.", nameof(stockLotId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Movement quantity must be positive.");
        }

        if (operationId == Guid.Empty || sourceCommandId == Guid.Empty)
        {
            throw new ArgumentException("Optional identifiers must be non-empty when supplied.");
        }

        StockLotId = stockLotId;
        Type = type;
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Inventory movements must use a mass, volume, or count unit.", nameof(unit));
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
        OccurredAt = occurredAt;
        OperationId = operationId;
        SourceCommandId = sourceCommandId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public Guid StockLotId { get; private set; }

    public InventoryMovementType Type { get; private set; }

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public decimal EnteredQuantity { get; private set; }

    public string EnteredUnit { get; private set; } = string.Empty;

    public string DisplayUnit { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public Guid? OperationId { get; private set; }

    public Guid? SourceCommandId { get; private set; }

    public string? Notes { get; private set; }
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public sealed class WorkOrderResource : TenantEntity
{
    private WorkOrderResource() { }
    public WorkOrderResource(Guid id, Guid workOrderId, WorkResourceType resourceType, string resourceName,
        decimal quantity, string unit, decimal unitCost, string currency, Guid? agriculturalInputId = null,
        Guid? stockLotId = null, string? resourceReference = null) : base(id)
    {
        if (workOrderId == Guid.Empty || agriculturalInputId == Guid.Empty || stockLotId == Guid.Empty)
            throw new ArgumentException("Work order is required and optional identifiers must be non-empty.");
        if (!Enum.IsDefined(resourceType)) throw new ArgumentOutOfRangeException(nameof(resourceType));
        if (stockLotId is not null && agriculturalInputId is null) throw new ArgumentException("A stock lot requires an agricultural input.");
        if (resourceType != WorkResourceType.Input && (agriculturalInputId is not null || stockLotId is not null))
            throw new ArgumentException("Only input resources may reference agricultural inputs and stock lots.");
        if (unitCost < 0) throw new ArgumentOutOfRangeException(nameof(unitCost));
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension == UnitDimension.Temperature || measured.CanonicalValue <= 0)
            throw new ArgumentException("A positive resource quantity is required.", nameof(quantity));
        WorkOrderId = workOrderId; ResourceType = resourceType;
        ResourceName = Text(resourceName, nameof(resourceName), 160);
        Quantity = measured.CanonicalValue; Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue; EnteredUnit = measured.EnteredUnit;
        UnitCost = unitCost; Currency = new CurrencyCode(currency).Value;
        AgriculturalInputId = agriculturalInputId; StockLotId = stockLotId;
        ResourceReference = Optional(resourceReference, 160); Status = WorkResourceStatus.Reserved;
    }
    public Guid WorkOrderId { get; private set; }
    public WorkResourceType ResourceType { get; private set; }
    public string ResourceName { get; private set; } = string.Empty;
    public string? ResourceReference { get; private set; }
    public Guid? AgriculturalInputId { get; private set; }
    public Guid? StockLotId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public decimal UnitCost { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public WorkResourceStatus Status { get; private set; }
    public decimal PlannedCost => decimal.Round(Quantity * UnitCost, 2, MidpointRounding.AwayFromZero);
    public void Consume() { if (Status != WorkResourceStatus.Reserved) throw new InvalidOperationException("Only reserved resources can be consumed."); Status = WorkResourceStatus.Consumed; }
    public void Release() { if (Status != WorkResourceStatus.Reserved) throw new InvalidOperationException("Only reserved resources can be released."); Status = WorkResourceStatus.Released; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

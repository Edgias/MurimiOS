namespace Edgias.MurimiOS.Domain.Entities;

public class Bin(string name, decimal capacity, Guid binTypeId,
        Guid unitGroupId, Guid unitMeasurementId, Guid warehouseId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public decimal Capacity { get; private set; } = capacity;

    public Guid BinTypeId { get; private set; } = binTypeId;

    public BinType BinType { get; private set; } = default!;

    public Guid UnitGroupId { get; private set; } = unitGroupId;

    public UnitGroup UnitGroup { get; private set; } = default!;

    public Guid UnitMeasurementId { get; private set; } = unitMeasurementId;

    public UnitMeasurement UnitMeasurement { get; private set; } = default!;

    public Guid WarehouseId { get; private set; } = warehouseId;

    public Warehouse Warehouse { get; private set; } = default!;

    public void Update(string name, decimal capacity)
    {
        Name = name;
        Capacity = capacity;
    }

    public void UpdateBinType(Guid binTypeId)
    {
        BinTypeId = binTypeId;
    }

    public void UpdateUnitGroup(Guid unitGroupId)
    {
        UnitGroupId = unitGroupId;
    }

    public void UpdateUnitMeasurement(Guid unitMeasurementId)
    {
        UnitMeasurementId = unitMeasurementId;
    }

    public void UpdateWarehouse(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }
}



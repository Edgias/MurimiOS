using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class Bin : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public decimal Capacity { get; private set; }

        public Guid BinTypeId { get; private set; }

        public BinType BinType { get; private set; }

        public Guid UnitGroupId { get; private set; }

        public UnitGroup UnitGroup { get; private set; }

        public Guid UnitMeasurementId { get; private set; }

        public UnitMeasurement UnitMeasurement { get; private set; }

        public Guid WarehouseId { get; private set; }

        public Warehouse Warehouse { get; private set; }

        public Bin(string name, decimal capacity, Guid binTypeId, Guid unitGroupId, Guid unitMeasurementId, Guid warehouseId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(capacity, nameof(capacity));
            Guard.AgainstNull(binTypeId, nameof(binTypeId));
            Guard.AgainstNull(unitGroupId, nameof(unitGroupId));
            Guard.AgainstNull(unitMeasurementId, nameof(unitMeasurementId));
            Guard.AgainstNull(warehouseId, nameof(warehouseId));

            Name = name;
            Capacity = capacity;
            BinTypeId = binTypeId;
            UnitGroupId = unitGroupId;
            UnitMeasurementId = unitMeasurementId;
            WarehouseId = warehouseId;
        }

        public void UpdateDetails(string name, decimal capacity)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(capacity, nameof(capacity));


            Name = name;
            Capacity = capacity;
        }

        public void UpdateBinType(Guid binTypeId)
        {
            Guard.AgainstNull(binTypeId, nameof(binTypeId));

            BinTypeId = binTypeId;
        }

        public void UpdateUnitGroup(Guid unitGroupId)
        {
            Guard.AgainstNull(unitGroupId, nameof(unitGroupId));

            UnitGroupId = unitGroupId;
        }

        public void UpdateUnitMeasurement(Guid unitMeasurementId)
        {
            Guard.AgainstNull(unitMeasurementId, nameof(unitMeasurementId));

            UnitMeasurementId = unitMeasurementId;
        }

        public void UpdateWarehouse(Guid warehouseId)
        {
            Guard.AgainstNull(warehouseId, nameof(warehouseId));

            WarehouseId = warehouseId;
        }
    }
}

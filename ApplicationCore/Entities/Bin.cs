using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Bin : BaseEntity
    {
        public string Name { get; set; }

        public decimal Capacity { get; set; }

        public Guid UnitGroupId { get; set; }

        public UnitGroup UnitGroup { get; set; }

        public Guid UnitMeasurementId { get; set; }

        public UnitMeasurement UnitMeasurement { get; set; }

        public Guid WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }
    }
}

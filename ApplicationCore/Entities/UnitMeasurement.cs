using System;

namespace Murimi.ApplicationCore.Entities
{
    public class UnitMeasurement : BaseEntity
    {
        public string Name { get; set; }

        public Guid UnitGroupId { get; set; }

        public UnitGroup UnitGroup { get; set; }
    }
}

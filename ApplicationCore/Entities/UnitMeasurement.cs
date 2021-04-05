using System;

namespace Edgias.Agrik.ApplicationCore.Entities
{
    public class UnitMeasurement : BaseEntity
    {
        public string Name { get; set; }

        public Guid UnitGroupId { get; set; }

        public UnitGroup UnitGroup { get; set; }
    }
}

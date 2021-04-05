using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Field : BaseEntity
    {
        public string Name { get; set; }

        public Guid? LocationId { get; set; }

        public Location Location { get; set; }

        public Guid FieldMeasurementId { get; set; }

        public FieldMeasurement FieldMeasurement { get; set; }

        public decimal UsableArea { get; set; }

        public Guid? SoilTypeId { get; set; }

        public SoilType SoilType { get; set; }

        public Guid? OwnershipTypeId { get; set; }

        public OwnershipType OwnershipType { get; set; }
    }
}

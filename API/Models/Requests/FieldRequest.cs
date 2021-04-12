using System;

namespace Murimi.API.Models.Requests
{
    public class FieldRequest
    {
        public string Name { get; set; }

        public Guid? LocationId { get; set; }

        public Guid FieldMeasurementId { get; set; }

        public decimal UsableArea { get; set; }

        public Guid? SoilTypeId { get; set; }

        public Guid? OwnershipTypeId { get; set; }
    }
}

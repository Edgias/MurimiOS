using System;

namespace Murimi.API.Models.Request
{
    public class FieldRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid? LocationId { get; set; }

        public Guid FieldMeasurementId { get; set; }

        public decimal UsableArea { get; set; }

        public Guid? SoilTypeId { get; set; }

        public Guid? OwnershipTypeId { get; set; }
    }
}

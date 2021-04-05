using System;

namespace Edgias.Agrik.API.Models.Form
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

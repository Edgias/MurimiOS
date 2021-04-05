using System;

namespace Murimi.API.Models.Response
{
    public class FieldApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid? LocationId { get; set; }

        public string Location { get; set; }

        public Guid FieldMeasurementId { get; set; }

        public string FieldMeasurement { get; set; }

        public decimal UsableArea { get; set; }

        public Guid? SoilTypeId { get; set; }

        public string SoilType { get; set; }

        public Guid? OwnershipTypeId { get; set; }

        public string OwnershipType { get; set; }
    }
}

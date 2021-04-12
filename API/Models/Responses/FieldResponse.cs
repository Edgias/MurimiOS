using System;

namespace Murimi.API.Models.Responses
{
    public class FieldResponse : BaseResponse
    {
        public string Name { get; set; }

        public Guid? LocationId { get; set; }

        public string LocationName { get; set; }

        public Guid FieldMeasurementId { get; set; }

        public string FieldMeasurementName { get; set; }

        public decimal UsableArea { get; set; }

        public Guid? SoilTypeId { get; set; }

        public string SoilTypeName { get; set; }

        public Guid? OwnershipTypeId { get; set; }

        public string OwnershipTypeName { get; set; }
    }
}

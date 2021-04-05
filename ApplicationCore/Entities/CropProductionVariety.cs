using System;

namespace Murimi.ApplicationCore.Entities
{
    public class CropProductionVariety : BaseEntity
    {
        public Guid CropProductionId { get; set; }

        public CropProduction CropProduction { get; set; }

        public Guid CropVarietyId { get; set; }

        public CropVariety CropVariety { get; set; }
    }
}

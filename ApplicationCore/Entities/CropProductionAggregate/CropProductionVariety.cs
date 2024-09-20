using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities.CropProductionAggregate
{
    public class CropProductionVariety : BaseEntity
    {
        public Guid CropProductionId { get; private set; }

        public CropProduction CropProduction { get; private set; }

        public Guid CropVarietyId { get; private set; }

        public CropVariety CropVariety { get; private set; }

        public CropProductionVariety(Guid cropProductionId, Guid cropVarientyId)
        {
            Guard.AgainstNull(cropProductionId, nameof(cropProductionId));
            Guard.AgainstNull(cropVarientyId, nameof(cropVarientyId));

            CropProductionId = cropProductionId;
            CropVarietyId = cropVarientyId;
        }
    }
}

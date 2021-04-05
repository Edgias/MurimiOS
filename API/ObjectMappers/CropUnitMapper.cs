using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class CropUnitMapper : IMapper<CropUnit, CropUnitRequestApiModel, CropUnitApiModel>
    {
        public CropUnit Map(CropUnitRequestApiModel apiModel)
        {
            CropUnit entity = new CropUnit
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public CropUnitApiModel Map(CropUnit entity)
        {
            CropUnitApiModel apiModel = new CropUnitApiModel
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate
            };

            return apiModel;
        }

        public void Map(CropUnit entity, CropUnitRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class SoilTypeMapper : IMapper<SoilType, SoilTypeRequestApiModel, SoilTypeApiModel>
    {
        public SoilType Map(SoilTypeRequestApiModel apiModel)
        {
            SoilType entity = new SoilType
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public SoilTypeApiModel Map(SoilType entity)
        {
            SoilTypeApiModel apiModel = new SoilTypeApiModel
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name
            };

            return apiModel;
        }

        public void Map(SoilType entity, SoilTypeRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

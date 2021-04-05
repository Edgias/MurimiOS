using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.API.ObjectMappers
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

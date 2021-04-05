using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.API.ObjectMappers
{
    public class SeasonStatusMapper : IMapper<SeasonStatus, SeasonStatusRequestApiModel, SeasonStatusApiModel>
    {
        public SeasonStatus Map(SeasonStatusRequestApiModel apiModel)
        {
            SeasonStatus entity = new SeasonStatus
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public SeasonStatusApiModel Map(SeasonStatus entity)
        {
            SeasonStatusApiModel apiModel = new SeasonStatusApiModel
            {
                IsDefault = entity.IsDefault,
                Id = entity.Id,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name
            };

            return apiModel;
        }

        public void Map(SeasonStatus entity, SeasonStatusRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.IsDefault = apiModel.IsDefault;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.API.ObjectMappers
{
    public class SeasonMapper : IMapper<Season, SeasonRequestApiModel, SeasonApiModel>
    {
        public Season Map(SeasonRequestApiModel apiModel)
        {
            Season entity = new Season
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public SeasonApiModel Map(Season entity)
        {
            SeasonApiModel apiModel = new SeasonApiModel
            {
                Id = entity.Id,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name,
                StartDate = entity.StartDate,
                Status = entity.SeasonStatus?.Name,
                StatusId = entity.SeasonStatusId
            };

            return apiModel;
        }

        public void Map(Season entity, SeasonRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.StartDate = apiModel.StartDate;
            entity.EndDate = apiModel.EndDate;
            entity.LastModifiedBy = apiModel.UserId;
            entity.SeasonStatusId = apiModel.StatusId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

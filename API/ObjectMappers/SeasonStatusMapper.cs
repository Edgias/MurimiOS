using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class SeasonStatusMapper : IMapper<SeasonStatus, SeasonStatusRequest, SeasonStatusResponse>
    {
        public SeasonStatus Map(SeasonStatusRequest request)
        {
            SeasonStatus entity = new(request.Name, request.IsDefault);

            return entity;
        }

        public SeasonStatusResponse Map(SeasonStatus entity)
        {
            SeasonStatusResponse response = new()
            {
                IsDefault = entity.IsDefault,
                Id = entity.Id,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return response;
        }

        public void Map(SeasonStatus entity, SeasonStatusRequest request)
        {
            entity.UpdateName(request.Name);
        }
    }
}

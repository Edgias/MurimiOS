using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class SeasonMapper : IMapper<Season, SeasonRequest, SeasonResponse>
    {
        public Season Map(SeasonRequest request)
        {
            Season entity = new(request.Name, request.StartDate, request.EndDate, request.StatusId);

            return entity;
        }

        public SeasonResponse Map(Season entity)
        {
            SeasonResponse response = new()
            {
                Id = entity.Id,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                Name = entity.Name,
                StartDate = entity.StartDate,
                Status = entity.SeasonStatus?.Name,
                StatusId = entity.SeasonStatusId
            };

            return response;
        }

        public void Map(Season entity, SeasonRequest request)
        {
            entity.UpdateDetails(request.Name, request.StartDate, request.EndDate);
        }
    }
}

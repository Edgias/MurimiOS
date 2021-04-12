using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class SoilTypeMapper : IMapper<SoilType, SoilTypeRequest, SoilTypeResponse>
    {
        public SoilType Map(SoilTypeRequest request)
        {
            SoilType entity = new(request.Name);

            return entity;
        }

        public SoilTypeResponse Map(SoilType entity)
        {
            SoilTypeResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return response;
        }

        public void Map(SoilType entity, SoilTypeRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

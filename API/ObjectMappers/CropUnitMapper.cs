using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class CropUnitMapper : IMapper<CropUnit, CropUnitRequest, CropUnitResponse>
    {
        public CropUnit Map(CropUnitRequest request)
        {
            CropUnit entity = new(request.Name);

            return entity;
        }

        public CropUnitResponse Map(CropUnit entity)
        {
            CropUnitResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive
            };

            return response;
        }

        public void Map(CropUnit entity, CropUnitRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

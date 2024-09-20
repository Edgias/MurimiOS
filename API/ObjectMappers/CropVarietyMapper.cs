using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class CropVarietyMapper : IMapper<CropVariety, CropVarietyRequest, CropVarietyResponse>
    {
        public CropVariety Map(CropVarietyRequest request)
        {
            CropVariety entity = new(request.Name, request.CropId);

            return entity;
        }

        public CropVarietyResponse Map(CropVariety entity)
        {
            CropVarietyResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                CropId = entity.CropId,
                CropName = entity.Crop?.Name,
                IsActive = entity.IsActive
            };

            return response;
        }

        public void Map(CropVariety entity, CropVarietyRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

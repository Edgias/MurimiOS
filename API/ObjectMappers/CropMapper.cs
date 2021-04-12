using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class CropMapper : IMapper<Crop, CropRequest, CropResponse>
    {
        public Crop Map(CropRequest request)
        {
            Crop entity = new(request.Name, request.CropCategoryId, request.CropUnitId);

            return entity;
        }

        public CropResponse Map(Crop entity)
        {
            CropResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                CropCategoryName = entity.CropCategory?.Name,
                CropCategoryId = entity.CropCategoryId,
                CropUnitName = entity.CropUnit?.Name,
                CropUnitId = entity.CropUnitId,
                IsActive = entity.IsActive
            };

            return response;
        }

        public void Map(Crop entity, CropRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

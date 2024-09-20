using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class CropCategoryMapper : IMapper<CropCategory, CropCategoryRequest, CropCategoryApiModel>
    {
        public CropCategory Map(CropCategoryRequest request)
        {
            CropCategory entity = new(request.Name, request.Description);

            return entity;
        }

        public CropCategoryApiModel Map(CropCategory entity)
        {
            CropCategoryApiModel response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };

            return response;
        }

        public void Map(CropCategory entity, CropCategoryRequest request)
        {
            entity.UpdateDetails(request.Name, request.Description);
        }
    }
}

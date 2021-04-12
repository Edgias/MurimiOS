using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class OwnershipTypeMapper : IMapper<OwnershipType, OwnershipTypeRequest, OwnershipTypeResponse>
    {
        public OwnershipType Map(OwnershipTypeRequest request)
        {
            OwnershipType entity = new(request.Name);

            return entity;
        }

        public OwnershipTypeResponse Map(OwnershipType entity)
        {
            OwnershipTypeResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return response;
        }

        public void Map(OwnershipType entity, OwnershipTypeRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

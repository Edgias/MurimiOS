using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class OwnershipTypeMapper : IMapper<OwnershipType, OwnershipTypeRequestApiModel, OwnershipTypeApiModel>
    {
        public OwnershipType Map(OwnershipTypeRequestApiModel apiModel)
        {
            OwnershipType entity = new OwnershipType
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public OwnershipTypeApiModel Map(OwnershipType entity)
        {
            OwnershipTypeApiModel apiModel = new OwnershipTypeApiModel
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name
            };

            return apiModel;
        }

        public void Map(OwnershipType entity, OwnershipTypeRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

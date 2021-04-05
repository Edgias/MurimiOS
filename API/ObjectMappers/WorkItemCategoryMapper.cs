using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class WorkItemCategoryMapper : IMapper<WorkItemCategory, WorkItemCategoryRequestApiModel, WorkItemCategoryApiModel>
    {
        public WorkItemCategory Map(WorkItemCategoryRequestApiModel apiModel)
        {
            WorkItemCategory entity = new WorkItemCategory
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public WorkItemCategoryApiModel Map(WorkItemCategory entity)
        {
            WorkItemCategoryApiModel apiModel = new WorkItemCategoryApiModel
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                IsActive = entity.IsActive,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name
            };

            return apiModel;
        }

        public void Map(WorkItemCategory entity, WorkItemCategoryRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

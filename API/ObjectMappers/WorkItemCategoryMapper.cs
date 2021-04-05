using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.API.ObjectMappers
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

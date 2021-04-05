using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class WorkItemSubCategoryMapper : IMapper<WorkItemSubCategory, WorkItemSubCategoryRequestApiModel, WorkItemSubCategoryApiModel>
    {
        public WorkItemSubCategory Map(WorkItemSubCategoryRequestApiModel apiModel)
        {
            WorkItemSubCategory entity = new WorkItemSubCategory
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public WorkItemSubCategoryApiModel Map(WorkItemSubCategory entity)
        {
            WorkItemSubCategoryApiModel apiModel = new WorkItemSubCategoryApiModel
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                WorkItemCategory = entity.WorkItemCategory?.Name,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name,
                WorkItemCategoryId = entity.WorkItemCategoryId
            };

            return apiModel;
        }

        public void Map(WorkItemSubCategory entity, WorkItemSubCategoryRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.WorkItemCategoryId = apiModel.WorkItemCategoryId;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

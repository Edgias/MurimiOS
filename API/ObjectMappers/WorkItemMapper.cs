using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class WorkItemMapper : IMapper<WorkItem, WorkItemRequestApiModel, WorkItemApiModel>
    {
        public WorkItem Map(WorkItemRequestApiModel apiModel)
        {
            WorkItem entity = new WorkItem
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public WorkItemApiModel Map(WorkItem entity)
        {
            WorkItemApiModel apiModel = new WorkItemApiModel
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                CropProductionId = entity.CropProductionId,
                FieldId = entity.FieldId,
                EndDate = entity.EndDate,
                CreatedDate = entity.CreatedDate,
                CropProduction = entity.CropProduction?.Name,
                WorkItemCategory = entity.WorkItemCategory?.Name,
                WorkItemCategoryId = entity.WorkItemCategoryId,
                WorkItemStatus = entity.WorkItemStatus?.Name,
                WorkItemStatusId = entity.WorkItemStatusId,
                WorkItemSubCategory = entity.WorkItemSubCategory?.Name,
                WorkItemSubCategoryId = entity.WorkItemSubCategoryId,
                Description = entity.Description,
                Field = entity.Field?.Name,
                LastModifiedDate = entity.LastModifiedDate,
                Name = entity.Name,
                SeasonId = entity.SeasonId,
                Season = entity.Season?.Name,
                StartDate = entity.StartDate
            };

            return apiModel;
        }

        public void Map(WorkItem entity, WorkItemRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.Description = apiModel.Description;
            entity.CropProductionId = apiModel.CropProductionId;
            entity.EndDate = apiModel.EndDate;
            entity.FieldId = apiModel.FieldId;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
            entity.SeasonId = apiModel.SeasonId;
            entity.StartDate = apiModel.StartDate;
            entity.WorkItemCategoryId = apiModel.WorkItemCategoryId;
            entity.WorkItemStatusId = apiModel.WorkItemStatusId;
            entity.WorkItemSubCategoryId = apiModel.WorkItemSubCategoryId;
        }
    }
}

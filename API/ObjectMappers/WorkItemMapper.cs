using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class WorkItemMapper : IMapper<WorkItem, WorkItemRequest, WorkItemResponse>
    {
        public WorkItem Map(WorkItemRequest request)
        {
            WorkItem entity = new(request.Name, request.Description, request.StartDate, request.EndDate, 
                request.WorkItemStatusId, request.WorkItemCategoryId, request.WorkItemSubCategoryId, request.FieldId, request.SeasonId, request.CropProductionId);

            return entity;
        }

        public WorkItemResponse Map(WorkItem entity)
        {
            WorkItemResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                CropProductionId = entity.CropProductionId,
                FieldId = entity.FieldId,
                EndDate = entity.EndDate,
                CropProductionName = entity.CropProduction?.Name,
                WorkItemCategoryName = entity.WorkItemCategory?.Name,
                WorkItemCategoryId = entity.WorkItemCategoryId,
                WorkItemStatusName = entity.WorkItemStatus?.Name,
                WorkItemStatusId = entity.WorkItemStatusId,
                WorkItemSubCategoryName = entity.WorkItemSubCategory?.Name,
                WorkItemSubCategoryId = entity.WorkItemSubCategoryId,
                Description = entity.Description,
                FieldName = entity.Field?.Name,
                Name = entity.Name,
                SeasonId = entity.SeasonId,
                SeasonName = entity.Season?.Name,
                StartDate = entity.StartDate
            };

            return response;
        }

        public void Map(WorkItem entity, WorkItemRequest request)
        {
            entity.UpdateDetails(request.Name, request.Description, request.StartDate, request.EndDate);
        }
    }
}

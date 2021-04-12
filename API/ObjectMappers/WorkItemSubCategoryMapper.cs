using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class WorkItemSubCategoryMapper : IMapper<WorkItemSubCategory, WorkItemSubCategoryRequest, WorkItemSubCategoryResponse>
    {
        public WorkItemSubCategory Map(WorkItemSubCategoryRequest request)
        {
            WorkItemSubCategory entity = new(request.Name, request.WorkItemCategoryId);

            return entity;
        }

        public WorkItemSubCategoryResponse Map(WorkItemSubCategory entity)
        {
            WorkItemSubCategoryResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                WorkItemCategoryName = entity.WorkItemCategory?.Name,
                Name = entity.Name,
                WorkItemCategoryId = entity.WorkItemCategoryId
            };

            return response;
        }

        public void Map(WorkItemSubCategory entity, WorkItemSubCategoryRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

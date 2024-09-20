using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class WorkItemCategoryMapper : IMapper<WorkItemCategory, WorkItemCategoryRequest, WorkItemCategoryResponse>
    {
        public WorkItemCategory Map(WorkItemCategoryRequest request)
        {
            WorkItemCategory entity = new(request.Name);

            return entity;
        }

        public WorkItemCategoryResponse Map(WorkItemCategory entity)
        {
            WorkItemCategoryResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return response;
        }

        public void Map(WorkItemCategory entity, WorkItemCategoryRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

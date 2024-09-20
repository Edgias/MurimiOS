using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class WorkItemStatusMapper : IMapper<WorkItemStatus, WorkItemStatusRequest, WorkItemStatusResponse>
    {
        public WorkItemStatus Map(WorkItemStatusRequest request)
        {
            WorkItemStatus entity = new(request.Name, request.IsDefault);

            return entity;
        }

        public WorkItemStatusResponse Map(WorkItemStatus entity)
        {
            WorkItemStatusResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault
            };

            return response;
        }

        public void Map(WorkItemStatus entity, WorkItemStatusRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

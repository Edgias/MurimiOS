using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class WorkItemStatusMapper : IMapper<WorkItemStatus, WorkItemStatusRequestApiModel, WorkItemStatusApiModel>
    {
        public WorkItemStatus Map(WorkItemStatusRequestApiModel apiModel)
        {
            WorkItemStatus entity = new WorkItemStatus
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public WorkItemStatusApiModel Map(WorkItemStatus entity)
        {
            WorkItemStatusApiModel apiModel = new WorkItemStatusApiModel
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive,
                IsDefault = entity.IsDefault,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate
            };

            return apiModel;
        }

        public void Map(WorkItemStatus entity, WorkItemStatusRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.IsDefault = apiModel.IsDefault;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

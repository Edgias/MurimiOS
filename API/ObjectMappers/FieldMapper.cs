using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.API.ObjectMappers
{
    public class FieldMapper : IMapper<Field, FieldRequestApiModel, FieldApiModel>
    {
        public Field Map(FieldRequestApiModel apiModel)
        {
            Field entity = new Field
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public FieldApiModel Map(Field entity)
        {
            FieldApiModel apiModel = new FieldApiModel
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                FieldMeasurement = entity.FieldMeasurement?.Name,
                FieldMeasurementId = entity.FieldMeasurementId,
                IsActive = entity.IsActive,
                LastModifiedDate = entity.LastModifiedDate,
                Location = entity.Location?.Name,
                LocationId = entity.LocationId,
                Name = entity.Name,
                OwnershipType = entity.OwnershipType?.Name,
                OwnershipTypeId = entity.OwnershipTypeId,
                SoilType = entity.SoilType?.Name,
                SoilTypeId = entity.SoilTypeId,
                UsableArea = entity.UsableArea
            };

            return apiModel;
        }

        public void Map(Field entity, FieldRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.FieldMeasurementId = apiModel.FieldMeasurementId;
            entity.LocationId = apiModel.LocationId;
            entity.OwnershipTypeId = apiModel.OwnershipTypeId;
            entity.SoilTypeId = apiModel.SoilTypeId;
            entity.UsableArea = apiModel.UsableArea;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

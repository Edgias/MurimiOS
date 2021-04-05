using Murimi.API.Interfaces;
using Murimi.API.Models.Request;
using Murimi.API.Models.Response;
using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.API.ObjectMappers
{
    public class FieldMeasurementMapper : IMapper<FieldMeasurement, FieldMeasurementRequestApiModel, FieldMeasurementApiModel>
    {
        public FieldMeasurement Map(FieldMeasurementRequestApiModel apiModel)
        {
            FieldMeasurement entity = new FieldMeasurement
            {
                CreatedBy = apiModel.UserId
            };

            Map(entity, apiModel);

            return entity;
        }

        public FieldMeasurementApiModel Map(FieldMeasurement entity)
        {
            FieldMeasurementApiModel apiModel = new FieldMeasurementApiModel
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return apiModel;
        }

        public void Map(FieldMeasurement entity, FieldMeasurementRequestApiModel apiModel)
        {
            entity.Name = apiModel.Name;
            entity.LastModifiedBy = apiModel.UserId;
            entity.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}

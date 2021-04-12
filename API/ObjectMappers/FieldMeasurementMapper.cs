using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class FieldMeasurementMapper : IMapper<FieldMeasurement, FieldMeasurementRequest, FieldMeasurementApiModel>
    {
        public FieldMeasurement Map(FieldMeasurementRequest request)
        {
            FieldMeasurement entity = new(request.Name);

            return entity;
        }

        public FieldMeasurementApiModel Map(FieldMeasurement entity)
        {
            FieldMeasurementApiModel response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                Name = entity.Name
            };

            return response;
        }

        public void Map(FieldMeasurement entity, FieldMeasurementRequest request)
        {
            entity.UpdateDetails(request.Name);
        }
    }
}

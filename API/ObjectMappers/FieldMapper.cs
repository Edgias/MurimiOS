using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Edgias.MurimiOS.Domain.Entities;

namespace Murimi.API.ObjectMappers
{
    public class FieldMapper : IMapper<Field, FieldRequest, FieldResponse>
    {
        public Field Map(FieldRequest request)
        {
            Field entity = new(request.Name, request.LocationId, request.FieldMeasurementId, 
                request.UsableArea, request.SoilTypeId, request.OwnershipTypeId);

            return entity;
        }

        public FieldResponse Map(Field entity)
        {
            FieldResponse response = new()
            {
                Id = entity.Id,
                FieldMeasurementName = entity.FieldMeasurement?.Name,
                FieldMeasurementId = entity.FieldMeasurementId,
                IsActive = entity.IsActive,
                LocationName = entity.Location?.Name,
                LocationId = entity.LocationId,
                Name = entity.Name,
                OwnershipTypeName = entity.OwnershipType?.Name,
                OwnershipTypeId = entity.OwnershipTypeId,
                SoilTypeName = entity.SoilType?.Name,
                SoilTypeId = entity.SoilTypeId,
                UsableArea = entity.UsableArea
            };

            return response;
        }

        public void Map(Field entity, FieldRequest request)
        {
            entity.UpdateName(request.Name);
        }
    }
}

namespace Edgias.MurimiOS.API.Fields;

public static class FieldMapper
{
    public static FieldResponse AsApiResponse(this Field entity)
    {
        FieldResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            FieldMeasurementId = entity.FieldMeasurementId,
            FieldMeasurementName = entity.FieldMeasurement?.Name!,
            LocationId = entity.LocationId,
            Location = entity.Location?.Name!,
            OwnershipTypeId = entity.OwnershipTypeId,
            OwnershipTypeName = entity.OwnershipType?.Name!,
            SoilTypeId = entity.SoilTypeId,
            SoilTypeName = entity.SoilType?.Name!,
            UsableArea = entity.UsableArea
        };

        return response;
    }

    public static Field ToEntity(this FieldRequest request)
    {
        Field entity = new(request.Name, request.LocationId, request.FieldMeasurementId,
            request.UsableArea, request.SoilTypeId, request.OwnershipTypeId);

        return entity;
    }

    public static void Update(this Field entity, FieldRequest request)
    {
        //entity.UpdateDetails(request.Name);
    }
}


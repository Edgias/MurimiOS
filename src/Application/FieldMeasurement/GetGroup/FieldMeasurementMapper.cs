namespace Edgias.MurimiOS.API.FieldMeasurements;

public static class FieldMeasurementMapper
{
    public static FieldMeasurementResponse AsApiResponse(this FieldMeasurement entity)
    {
        FieldMeasurementResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static FieldMeasurement ToEntity(this FieldMeasurementRequest request)
    {
        FieldMeasurement entity = new(request.Name);

        return entity;
    }

    public static void Update(this FieldMeasurement entity, FieldMeasurementRequest request)
    {
        entity.Update(request.Name);
    }
}


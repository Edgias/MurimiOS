namespace Edgias.MurimiOS.API.YieldMeasurements;

public static class YieldMeasurementMapper
{
    public static YieldMeasurementResponse AsApiResponse(this YieldMeasurement entity)
    {
        YieldMeasurementResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static YieldMeasurement ToEntity(this YieldMeasurementRequest request)
    {
        YieldMeasurement entity = new(request.Name);

        return entity;
    }

    public static void Update(this YieldMeasurement entity, YieldMeasurementRequest request)
    {
        entity.UpdateDetails(request.Name);
    }
}


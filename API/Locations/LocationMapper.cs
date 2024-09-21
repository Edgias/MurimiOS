namespace Edgias.MurimiOS.API.Locations;

public static class LocationMapper
{
    public static LocationResponse AsApiResponse(this Location entity)
    {
        LocationResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            LocationAddress = entity.LocationAddress,
        };

        return response;
    }

    public static Location ToEntity(this LocationRequest request)
    {
        Location entity = new(request.Name, request.Latitude,request.Longitude,
            request.LocationAddress);

        return entity;
    }

    public static void Update(this Location entity, LocationRequest request)
    {
        entity.Update(request.Name, request.Latitude, request.Longitude,
            request.LocationAddress);
    }
}


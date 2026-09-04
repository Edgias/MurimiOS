namespace Edgias.MurimiOS.API.SeasonStatuses;

public static class SeasonStatusMapper
{
    public static SeasonStatusResponse AsApiResponse(this SeasonStatus entity)
    {
        SeasonStatusResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDefault = entity.IsDefault
        };

        return response;
    }

    public static SeasonStatus ToEntity(this SeasonStatusRequest request)
    {
        SeasonStatus entity = new(request.Name, request.IsDefault);

        return entity;
    }

    public static void Update(this SeasonStatus entity, SeasonStatusRequest request)
    {
        entity.Update(request.Name);
    }
}


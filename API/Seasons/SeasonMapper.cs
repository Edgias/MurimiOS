namespace Edgias.MurimiOS.API.Seasons;

public static class SeasonMapper
{
    public static SeasonResponse AsApiResponse(this Season entity)
    {
        SeasonResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            SeasonStatusId = entity.SeasonStatusId,
            SeasonStatusName = entity.SeasonStatus?.Name!,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };

        return response;
    }

    public static Season ToEntity(this SeasonRequest request)
    {
        Season entity = new(request.Name, request.StartDate, request.EndDate);

        return entity;
    }

    public static void Update(this Season entity, SeasonRequest request)
    {
        entity.Update(request.Name, request.StartDate, request.EndDate);
    }
}


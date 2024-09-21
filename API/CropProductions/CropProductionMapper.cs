namespace Edgias.MurimiOS.API.CropProductions;

public static class CropProductionMapper
{
    public static CropProductionResponse AsApiResponse(this CropProduction entity)
    {
        CropProductionResponse response = new()
        {
            Id = entity.Id,
            ExpectedYield = entity.ExpectedYield,
            ActualYield = entity.ActualYield,
            Name = entity.Name,
            CropId = entity.CropId,
            CropName = entity.Crop?.Name!,
            SeasonId = entity.SeasonId,
            SeasonName = entity.Season?.Name!
        };

        return response;
    }

    public static CropProduction ToEntity(this CropProductionRequest request)
    {
        CropProduction entity = new(request.Name, request.CropId, request.SeasonId, request.ExpectedYield, request.ActualYield);

        return entity;
    }

    public static void Update(this CropProduction entity, CropProductionRequest request)
    {
        entity.UpdateDetails(request.Name, request.ExpectedYield, request.ActualYield);
    }
}


namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared;

public record CropProductionResponse : CropProductionModel
{
    public Guid Id { get; set; }

    public string CropName { get; set; } = default!;

    public string SeasonName { get; set; } = default!;

}


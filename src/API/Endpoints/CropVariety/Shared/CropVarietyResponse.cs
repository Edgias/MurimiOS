namespace Edgias.MurimiOS.API.CropVarieties;

public record CropVarietyResponse : CropVarietyModel
{
    public Guid Id { get; set; }

    public string CropName { get; set; } = default!;

}


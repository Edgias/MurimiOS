using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared;

public record CropUnitModel
{
    [Required]
    public string Name { get; set; } = default!;

}


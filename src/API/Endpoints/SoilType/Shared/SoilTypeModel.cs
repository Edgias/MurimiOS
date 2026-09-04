using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared;

public record SoilTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}


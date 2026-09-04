using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.SoilTypes;

public record SoilTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}


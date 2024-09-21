using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.CropUnits;

public record CropUnitModel
{
    [Required]
    public string Name { get; set; } = default!;

}


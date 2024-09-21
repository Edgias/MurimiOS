using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.CropVarieties;

public record CropVarietyModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropId { get; set; }
}


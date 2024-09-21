using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Crops;

public record CropResponse : CropModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string CropCategoryName { get; set; } = default!;

    [Required]
    public string CropUnitName { get; set; } = default!;

}


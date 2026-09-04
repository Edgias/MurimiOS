using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Crop.Shared;

public record CropModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropCategoryId { get; set; }

    [Required]
    public Guid CropUnitId { get; set; }

}


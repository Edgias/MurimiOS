using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;

public record CropCategoryModel 
{
    [Required]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;
}

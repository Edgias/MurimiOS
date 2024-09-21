using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.CropCategories;

public record CropCategoryModel 
{
    [Required]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;
}

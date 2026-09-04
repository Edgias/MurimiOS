using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.ItemCategories;

public record ItemCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


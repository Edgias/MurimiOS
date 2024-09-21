using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemCategories;

public record WorkItemCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


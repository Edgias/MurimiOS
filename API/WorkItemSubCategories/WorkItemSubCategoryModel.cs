using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemSubCategories;

public record WorkItemSubCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid WorkItemCategoryId { get; set; }

}


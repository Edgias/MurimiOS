using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemSubCategories;

public record WorkItemSubCategoryResponse : WorkItemSubCategoryModel
{
    [Required]
    public Guid Id { get; set; }

    public string WorkItemCategoryName { get; set; } = default!;

}


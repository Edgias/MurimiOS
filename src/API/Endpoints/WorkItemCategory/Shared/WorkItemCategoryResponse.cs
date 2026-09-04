using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemCategories;

public record WorkItemCategoryResponse : WorkItemCategoryModel
{
    [Required]
    public Guid Id { get; set; }

}


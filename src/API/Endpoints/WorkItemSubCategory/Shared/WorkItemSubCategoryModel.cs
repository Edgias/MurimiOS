using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared;

public record WorkItemSubCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid WorkItemCategoryId { get; set; }

}


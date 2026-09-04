using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared;

public record WorkItemSubCategoryResponse : WorkItemSubCategoryModel
{
    [Required]
    public Guid Id { get; set; }

    public string WorkItemCategoryName { get; set; } = default!;

}


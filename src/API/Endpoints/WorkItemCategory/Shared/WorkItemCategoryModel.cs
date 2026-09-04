using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;

public record WorkItemCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


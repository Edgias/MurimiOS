using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;

public record WorkItemCategoryResponse : WorkItemCategoryModel
{
    [Required]
    public Guid Id { get; set; }

}


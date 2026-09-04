using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;

public record ItemCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


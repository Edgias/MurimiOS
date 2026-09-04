using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared;

public record EquipmentCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


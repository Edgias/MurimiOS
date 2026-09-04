using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.EquipmentCategories;

public record EquipmentCategoryModel
{
    [Required]
    public string Name { get; set; } = default!;

}


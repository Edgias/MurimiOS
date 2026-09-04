using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.OwnershipTypes;

public record OwnershipTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}


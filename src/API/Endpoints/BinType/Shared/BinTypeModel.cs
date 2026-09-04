using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.BinTypes;

public record BinTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}


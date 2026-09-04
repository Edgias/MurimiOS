using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.BinTypes;

public record BinTypeResponse : BinTypeModel
{
    [Required]
    public Guid Id { get; set; }

}


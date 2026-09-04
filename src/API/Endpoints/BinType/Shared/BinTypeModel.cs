using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;

public record BinTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}


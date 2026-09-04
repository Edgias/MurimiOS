using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;

public record BinTypeResponse : BinTypeModel
{
    [Required]
    public Guid Id { get; set; }

}


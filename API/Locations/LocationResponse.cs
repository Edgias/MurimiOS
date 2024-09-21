using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Locations;

public record LocationResponse : LocationModel
{
    [Required]
    public Guid Id { get; set; }

}


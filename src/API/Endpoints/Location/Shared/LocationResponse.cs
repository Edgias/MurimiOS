using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Location.Shared;

public record LocationResponse : LocationModel
{
    [Required]
    public Guid Id { get; set; }

}


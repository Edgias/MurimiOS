using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Location.Shared;

public record LocationModel
{
    [Required]
    public string Name { get; set; } = default!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public Address LocationAddress { get; set; } = default!;

}


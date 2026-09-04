using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;

public record SeasonStatusModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public bool IsDefault { get; set; }

}


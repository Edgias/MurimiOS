using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.SeasonStatuses;

public record SeasonStatusModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public bool IsDefault { get; set; }

}


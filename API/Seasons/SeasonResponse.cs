using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Seasons;

public record SeasonResponse : SeasonModel
{
    [Required]
    public Guid Id { get; set; }

    public string SeasonStatusName { get; set; } = default!;

}


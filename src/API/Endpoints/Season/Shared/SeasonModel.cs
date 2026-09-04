using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Seasons;

public record SeasonModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public DateTimeOffset StartDate { get; set; }

    [Required]
    public DateTimeOffset EndDate { get; set; }

    [Required]
    public Guid SeasonStatusId { get; set; }

}


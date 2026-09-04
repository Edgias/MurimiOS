using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.CropProductions;

public record CropProductionModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropId { get; set; }

    [Required]
    public Guid SeasonId { get; set; }

    [Required]
    public decimal ExpectedYield { get; set; }

    [Required]
    public decimal? ActualYield { get; set; }

}


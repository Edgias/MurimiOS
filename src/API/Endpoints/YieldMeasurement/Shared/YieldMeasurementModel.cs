using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.YieldMeasurements;

public record YieldMeasurementModel
{
    [Required]
    public string Name { get; set; } = default!;

}


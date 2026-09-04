using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;

public record YieldMeasurementModel
{
    [Required]
    public string Name { get; set; } = default!;

}


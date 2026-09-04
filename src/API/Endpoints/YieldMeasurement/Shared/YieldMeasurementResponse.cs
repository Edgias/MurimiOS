using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.YieldMeasurements;

public record YieldMeasurementResponse : YieldMeasurementModel
{
    [Required]
    public Guid Id { get; set; }

}


using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;

public record YieldMeasurementResponse : YieldMeasurementModel
{
    [Required]
    public Guid Id { get; set; }

}


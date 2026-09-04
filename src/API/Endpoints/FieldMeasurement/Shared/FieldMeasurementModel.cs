using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;

public record FieldMeasurementModel
{
    [Required]
    public string Name { get; set; } = default!;

}


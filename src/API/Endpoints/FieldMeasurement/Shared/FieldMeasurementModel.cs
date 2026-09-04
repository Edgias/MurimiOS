using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.FieldMeasurements;

public record FieldMeasurementModel
{
    [Required]
    public string Name { get; set; } = default!;

}


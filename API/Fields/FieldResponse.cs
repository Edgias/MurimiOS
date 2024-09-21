using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Fields;

public record FieldResponse : FieldModel
{
    [Required]
    public Guid Id { get; set; }

    public string Location { get; set; } = default!;

    public string FieldMeasurementName { get; set; } = default!;

    public string SoilTypeName { get; set; } = default!;

    public string OwnershipTypeName { get; set; } = default!;

}


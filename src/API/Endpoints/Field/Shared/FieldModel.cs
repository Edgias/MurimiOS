using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Fields;

public record FieldModel
{
    [Required]
    public string Name { get; set; } = default!;

    public Guid? LocationId { get; set; }

    [Required]
    public Guid FieldMeasurementId { get; set; }

    [Required]
    public decimal UsableArea { get; set; }

    public Guid? SoilTypeId { get; set; }

    public Guid? OwnershipTypeId { get; set; }

}


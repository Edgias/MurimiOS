using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Bins.Shared;

public record BinModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public decimal Capacity { get; set; }

    [Required]
    public Guid BinTypeId { get; set; }

    public Guid UnitGroupId { get; set; }

    [Required]
    public Guid UnitMeasurementId { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }
}

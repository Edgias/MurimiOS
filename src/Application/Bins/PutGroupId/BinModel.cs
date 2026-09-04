using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Bins.PutGroupId;

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

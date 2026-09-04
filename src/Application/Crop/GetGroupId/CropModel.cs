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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Crop.GetGroupId;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropCategory = global::EdgIQ.MurimiOS.Domain.CropCategory.CropCategory;
using CropUnit = global::EdgIQ.MurimiOS.Domain.CropUnit.CropUnit;

public record CropModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropCategoryId { get; set; }

    [Required]
    public Guid CropUnitId { get; set; }

}

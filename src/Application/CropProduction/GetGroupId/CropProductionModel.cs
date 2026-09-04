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
namespace EdgIQ.MurimiOS.Application.CropProduction.GetGroupId;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;

public record CropProductionModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropId { get; set; }

    [Required]
    public Guid SeasonId { get; set; }

    [Required]
    public decimal ExpectedYield { get; set; }

    [Required]
    public decimal? ActualYield { get; set; }

}

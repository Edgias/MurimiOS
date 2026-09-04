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
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Location.PutGroupId;
using Address = global::EdgIQ.MurimiOS.Domain.Common.Address;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

public record LocationModel
{
    [Required]
    public string Name { get; set; } = default!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public Address LocationAddress { get; set; } = default!;

}

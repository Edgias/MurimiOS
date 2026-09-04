using System.ComponentModel.DataAnnotations;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.Dispatcher;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Application.Bins.PutGroupId;
    public record BinResponse : BinModel
    {
        public Guid Id { get; set; }

        public string BinTypeName { get; set; } = default!;

        public string UnitMeasurementName { get; set; } = default!;

        public string WarehouseName { get; set; } = default!;
    }

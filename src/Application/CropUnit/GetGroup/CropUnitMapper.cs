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
using EdgIQ.MurimiOS.Domain.CropUnit;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropUnit.GetGroup;
using CropUnit = global::EdgIQ.MurimiOS.Domain.CropUnit.CropUnit;
    public static class CropUnitMapper
    {
        public static CropUnitResponse AsApiResponse(this CropUnit entity)
        {
            CropUnitResponse response = new()
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return response;
        }

        public static CropUnit ToEntity(this CropUnitRequest request)
        {
            CropUnit entity = new(request.Name);

            return entity;
        }

        public static void Update(this CropUnit entity, CropUnitRequest request)
        {
            entity.Update(request.Name);
        }
    }

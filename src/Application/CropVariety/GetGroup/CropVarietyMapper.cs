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
using EdgIQ.MurimiOS.Domain.CropVariety;
using EdgIQ.MurimiOS.Domain.Crop;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropVariety.GetGroup;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropVariety = global::EdgIQ.MurimiOS.Domain.CropVariety.CropVariety;

public static class CropVarietyMapper
{
    public static CropVarietyResponse AsApiResponse(this CropVariety entity)
    {
        CropVarietyResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropId = entity.CropId,
            CropName = entity.Crop?.Name!
        };

        return response;
    }

    public static CropVariety ToEntity(this CropVarietyRequest request)
    {
        CropVariety entity = new(request.Name, request.CropId);

        return entity;
    }

    public static void Update(this CropVariety entity, CropVarietyRequest request)
    {
        entity.Update(request.Name);
    }
}

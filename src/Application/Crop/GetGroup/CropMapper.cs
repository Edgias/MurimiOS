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
using EdgIQ.MurimiOS.Domain.Crop;
using EdgIQ.MurimiOS.Domain.CropCategory;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Crop.GetGroup;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropCategory = global::EdgIQ.MurimiOS.Domain.CropCategory.CropCategory;
using CropUnit = global::EdgIQ.MurimiOS.Domain.CropUnit.CropUnit;

public static class CropMapper
{
    public static CropResponse AsApiResponse(this Crop entity)
    {
        CropResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropCategoryId = entity.CropCategoryId,
            CropCategoryName = entity.CropCategory?.Name!,
            CropUnitId = entity.CropUnitId,
            CropUnitName = entity.CropUnit?.Name!
        };

        return response;
    }

    public static Crop ToEntity(this CropRequest request)
    {
        Crop entity = new(request.Name, request.CropCategoryId, request.CropUnitId);

        return entity;
    }

    public static void Update(this Crop entity, CropRequest request)
    {
        entity.UpdateName(request.Name);
    }
}

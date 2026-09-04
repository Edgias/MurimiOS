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
using EdgIQ.MurimiOS.Domain.CropCategory;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropCategory.PostGroup;
using CropCategory = global::EdgIQ.MurimiOS.Domain.CropCategory.CropCategory;

public static class CropCategoryMapper
{
    public static CropCategoryResponse AsApiResponse(this CropCategory entity)
    {
        CropCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };

        return response;
    }

    public static CropCategory ToEntity(this CropCategoryRequest request) 
    {
        CropCategory entity = new(request.Name, request.Description);

        return entity;
    }

    public static void Update(this CropCategory entity, CropCategoryRequest request)
    {
        entity.Update(request.Name, request.Description);
    }
}

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
using EdgIQ.MurimiOS.Domain.EquipmentCategory;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EquipmentCategory.GetGroup;
using EquipmentCategory = global::EdgIQ.MurimiOS.Domain.EquipmentCategory.EquipmentCategory;

public static class EquipmentCategoryMapper
{
    public static EquipmentCategoryResponse AsApiResponse(this EquipmentCategory entity)
    {
        EquipmentCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static EquipmentCategory ToEntity(this EquipmentCategoryRequest request)
    {
        EquipmentCategory entity = new(request.Name);

        return entity;
    }

    public static void Update(this EquipmentCategory entity, EquipmentCategoryRequest request)
    {
        entity.Update(request.Name);
    }
}

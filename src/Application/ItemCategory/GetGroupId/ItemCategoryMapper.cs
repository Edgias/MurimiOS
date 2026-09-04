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
using EdgIQ.MurimiOS.Domain.ItemCategory;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ItemCategory.GetGroupId;
using ItemCategory = global::EdgIQ.MurimiOS.Domain.ItemCategory.ItemCategory;

public static class ItemCategoryMapper
{
    public static ItemCategoryResponse AsApiResponse(this ItemCategory entity)
    {
        ItemCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static ItemCategory ToEntity(this ItemCategoryRequest request)
    {
        ItemCategory entity = new(request.Name);

        return entity;
    }

    public static void Update(this  ItemCategory entity, ItemCategoryRequest request) 
    {
        entity.Update(request.Name);
    }
}

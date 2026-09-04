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
using EdgIQ.MurimiOS.Domain.WorkItemSubCategory;
using EdgIQ.MurimiOS.Domain.WorkItemCategory;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkItemSubCategory.GetGroup;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;

public static class WorkItemSubCategoryMapper
{
    public static WorkItemSubCategoryResponse AsApiResponse(this WorkItemSubCategory entity)
    {
        WorkItemSubCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            WorkItemCategoryId = entity.WorkItemCategoryId,
            WorkItemCategoryName = entity.WorkItemCategory?.Name!
        };

        return response;
    }

    public static WorkItemSubCategory ToEntity(this WorkItemSubCategoryRequest request)
    {
        WorkItemSubCategory entity = new(request.Name, request.WorkItemCategoryId);

        return entity;
    }

    public static void Update(this WorkItemSubCategory entity, WorkItemSubCategoryRequest request)
    {
        entity.Update(request.Name);
    }
}

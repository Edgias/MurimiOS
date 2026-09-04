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
using EdgIQ.MurimiOS.Domain.WorkItemStatus;
using EdgIQ.MurimiOS.Domain.WorkItemCategory;
using EdgIQ.MurimiOS.Domain.WorkItem;
using EdgIQ.MurimiOS.Domain.Season;
using EdgIQ.MurimiOS.Domain.Field;
using EdgIQ.MurimiOS.Domain.CropProduction;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkItem.GetGroup;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;
using WorkItem = global::EdgIQ.MurimiOS.Domain.WorkItem.WorkItem;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;
using WorkItemStatus = global::EdgIQ.MurimiOS.Domain.WorkItemStatus.WorkItemStatus;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;

public static class WorkItemMapper
{
    public static WorkItemResponse AsApiResponse(this WorkItem entity)
    {
        WorkItemResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropProductionId = entity.CropProductionId,
            CropProductionName = entity.CropProduction?.Name!,
            WorkItemCategoryId = entity.WorkItemCategoryId,
            WorkItemCategoryName = entity.WorkItemCategory?.Name!,
            Description = entity.Description,
            EndDate = entity.EndDate,
            SeasonId = entity.SeasonId,
            SeasonName = entity.Season?.Name!,
            StartDate = entity.StartDate,
            WorkItemStatusId = entity.WorkItemStatusId,
            WorkItemStatusName = entity.WorkItemStatus?.Name!,
            WorkItemSubCategoryId = entity.WorkItemSubCategoryId,
            WorkItemSubCategoryName = entity.WorkItemSubCategory?.Name!,
            FieldId = entity.FieldId,
            FieldName = entity.Field?.Name!
        };

        return response;
    }

    public static WorkItem ToEntity(this WorkItemRequest request)
    {
        WorkItem entity = new(request.Name, request.Description, request.StartDate, request.EndDate, request.WorkItemCategoryId,
            request.WorkItemSubCategoryId, request.FieldId, request.SeasonId, request.CropProductionId);

        return entity;
    }

    public static void Update(this WorkItem entity, WorkItemRequest request)
    {
        entity.Update(request.Name, request.Description, request.StartDate, request.EndDate);
    }
}

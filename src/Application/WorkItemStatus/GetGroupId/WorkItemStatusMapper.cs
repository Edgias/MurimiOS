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
using EdgIQ.MurimiOS.Domain.WorkItemStatus;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkItemStatus.GetGroupId;
using WorkItemStatus = global::EdgIQ.MurimiOS.Domain.WorkItemStatus.WorkItemStatus;

public static class WorkItemStatusMapper
{
    public static WorkItemStatusResponse AsApiResponse(this WorkItemStatus entity)
    {
        WorkItemStatusResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDefault = entity.IsDefault
        };

        return response;
    }

    public static WorkItemStatus ToEntity(this WorkItemStatusRequest request)
    {
        WorkItemStatus entity = new(request.Name, request.IsDefault);

        return entity;
    }

    public static void Update(this WorkItemStatus entity, WorkItemStatusRequest request)
    {
        entity.Update(request.Name);
    }
}

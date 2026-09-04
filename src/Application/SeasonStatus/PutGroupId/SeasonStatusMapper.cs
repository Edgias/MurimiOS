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
using EdgIQ.MurimiOS.Domain.SeasonStatus;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SeasonStatus.PutGroupId;
using SeasonStatus = global::EdgIQ.MurimiOS.Domain.SeasonStatus.SeasonStatus;

public static class SeasonStatusMapper
{
    public static SeasonStatusResponse AsApiResponse(this SeasonStatus entity)
    {
        SeasonStatusResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDefault = entity.IsDefault
        };

        return response;
    }

    public static SeasonStatus ToEntity(this SeasonStatusRequest request)
    {
        SeasonStatus entity = new(request.Name, request.IsDefault);

        return entity;
    }

    public static void Update(this SeasonStatus entity, SeasonStatusRequest request)
    {
        entity.Update(request.Name);
    }
}

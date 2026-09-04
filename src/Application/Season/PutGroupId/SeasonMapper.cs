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
using EdgIQ.MurimiOS.Domain.Season;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Season.PutGroupId;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;
using SeasonStatus = global::EdgIQ.MurimiOS.Domain.SeasonStatus.SeasonStatus;

public static class SeasonMapper
{
    public static SeasonResponse AsApiResponse(this Season entity)
    {
        SeasonResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            SeasonStatusId = entity.SeasonStatusId,
            SeasonStatusName = entity.SeasonStatus?.Name!,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };

        return response;
    }

    public static Season ToEntity(this SeasonRequest request)
    {
        Season entity = new(request.Name, request.StartDate, request.EndDate);

        return entity;
    }

    public static void Update(this Season entity, SeasonRequest request)
    {
        entity.Update(request.Name, request.StartDate, request.EndDate);
    }
}

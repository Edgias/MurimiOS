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
using EdgIQ.MurimiOS.Domain.YieldMeasurement;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.YieldMeasurement.GetGroupId;
using YieldMeasurement = global::EdgIQ.MurimiOS.Domain.YieldMeasurement.YieldMeasurement;

public static class YieldMeasurementMapper
{
    public static YieldMeasurementResponse AsApiResponse(this YieldMeasurement entity)
    {
        YieldMeasurementResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static YieldMeasurement ToEntity(this YieldMeasurementRequest request)
    {
        YieldMeasurement entity = new(request.Name);

        return entity;
    }

    public static void Update(this YieldMeasurement entity, YieldMeasurementRequest request)
    {
        entity.UpdateDetails(request.Name);
    }
}

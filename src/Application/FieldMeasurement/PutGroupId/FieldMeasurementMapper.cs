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
using EdgIQ.MurimiOS.Domain.FieldMeasurement;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldMeasurement.PutGroupId;
using FieldMeasurement = global::EdgIQ.MurimiOS.Domain.FieldMeasurement.FieldMeasurement;

public static class FieldMeasurementMapper
{
    public static FieldMeasurementResponse AsApiResponse(this FieldMeasurement entity)
    {
        FieldMeasurementResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static FieldMeasurement ToEntity(this FieldMeasurementRequest request)
    {
        FieldMeasurement entity = new(request.Name);

        return entity;
    }

    public static void Update(this FieldMeasurement entity, FieldMeasurementRequest request)
    {
        entity.Update(request.Name);
    }
}

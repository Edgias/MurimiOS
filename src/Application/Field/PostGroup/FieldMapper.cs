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
using EdgIQ.MurimiOS.Domain.SoilType;
using EdgIQ.MurimiOS.Domain.FieldMeasurement;
using EdgIQ.MurimiOS.Domain.Field;
using EdgIQ.MurimiOS.Domain.OwnershipType;
using EdgIQ.MurimiOS.Domain.Location;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Field.PostGroup;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;
using FieldMeasurement = global::EdgIQ.MurimiOS.Domain.FieldMeasurement.FieldMeasurement;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;
using OwnershipType = global::EdgIQ.MurimiOS.Domain.OwnershipType.OwnershipType;
using SoilType = global::EdgIQ.MurimiOS.Domain.SoilType.SoilType;

public static class FieldMapper
{
    public static FieldResponse AsApiResponse(this Field entity)
    {
        FieldResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            FieldMeasurementId = entity.FieldMeasurementId,
            FieldMeasurementName = entity.FieldMeasurement?.Name!,
            LocationId = entity.LocationId,
            Location = entity.Location?.Name!,
            OwnershipTypeId = entity.OwnershipTypeId,
            OwnershipTypeName = entity.OwnershipType?.Name!,
            SoilTypeId = entity.SoilTypeId,
            SoilTypeName = entity.SoilType?.Name!,
            UsableArea = entity.UsableArea
        };

        return response;
    }

    public static Field ToEntity(this FieldRequest request)
    {
        Field entity = new(request.Name, request.LocationId, request.FieldMeasurementId,
            request.UsableArea, request.SoilTypeId, request.OwnershipTypeId);

        return entity;
    }

    public static void Update(this Field entity, FieldRequest request)
    {
        //entity.UpdateDetails(request.Name);
    }
}

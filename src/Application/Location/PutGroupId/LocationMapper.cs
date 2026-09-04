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
using EdgIQ.MurimiOS.Domain.Location;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Location.PutGroupId;
using Address = global::EdgIQ.MurimiOS.Domain.Common.Address;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

public static class LocationMapper
{
    public static LocationResponse AsApiResponse(this Location entity)
    {
        LocationResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            LocationAddress = entity.LocationAddress,
        };

        return response;
    }

    public static Location ToEntity(this LocationRequest request)
    {
        Location entity = new(request.Name, request.Latitude,request.Longitude,
            request.LocationAddress);

        return entity;
    }

    public static void Update(this Location entity, LocationRequest request)
    {
        entity.Update(request.Name, request.Latitude, request.Longitude,
            request.LocationAddress);
    }
}

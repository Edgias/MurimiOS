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
using EdgIQ.MurimiOS.Domain.NumberSequence;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.NumberSequence.PutGroupId;
using NumberSequence = global::EdgIQ.MurimiOS.Domain.NumberSequence.NumberSequence;

public static class NumberSequenceMapper
{
    public static NumberSequenceResponse AsApiResponse(this NumberSequence entity)
    {
        NumberSequenceResponse response = new()
        {
            Id = entity.Id,
            Entity = entity.Entity,
            Prefix = entity.Prefix,
            Seperator = entity.Seperator,
            Suffix = entity.Suffix,
            StartingNumber = entity.StartingNumber
        };

        return response;
    }

    public static NumberSequence ToEntity(this NumberSequenceRequest request)
    {
        NumberSequence entity = new(request.Entity, request.Prefix, request.Seperator, request.StartingNumber, request.Suffix);

        return entity;
    }

    public static void Update(this  NumberSequence entity, NumberSequenceRequest request) 
    {
        entity.Update(request.Prefix, request.Seperator, request.Suffix);
    }
}

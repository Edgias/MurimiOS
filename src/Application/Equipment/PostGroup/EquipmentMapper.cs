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
using EdgIQ.MurimiOS.Domain.Equipment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Equipment.PostGroup;
using Equipment = global::EdgIQ.MurimiOS.Domain.Equipment.Equipment;

public static class EquipmentMapper
{
    public static EquipmentResponse AsApiResponse(this Equipment entity)
    {
        EquipmentResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static Equipment ToEntity(this EquipmentRequest request)
    {
        Equipment entity = new(request.Name, request.Manufacturer, request.Model, request.RegistrationNumber, request.Year, request.Description, request.EquipmentCategoryId);

        return entity;
    }

    public static void Update(this Equipment entity, EquipmentRequest request)
    {
        entity.Update(request.Name, request.Manufacturer, request.Model, request.RegistrationNumber, request.Year, request.Description);
    }
}

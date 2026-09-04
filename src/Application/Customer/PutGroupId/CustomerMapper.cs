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
using EdgIQ.MurimiOS.Domain.Customer;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Customer.PutGroupId;
using Address = global::EdgIQ.MurimiOS.Domain.Common.Address;
using Customer = global::EdgIQ.MurimiOS.Domain.Customer.Customer;

public static class CustomerMapper
{
    public static CustomerResponse AsApiResponse(this Customer entity)
    {
        CustomerResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            ContactPerson = entity.ContactPerson,
            ContactPersonEmail = entity.ContactPersonEmail,
            ContactPersonPhone = entity.ContactPersonPhone,
            Website = entity.Website,
            BillingAddress = entity.BillingAddress
        };

        return response;
    }

    public static Customer ToEntity(this CustomerRequest request)
    {
        Customer entity = new(request.Name, request.Phone, request.Email,
            request.Website, request.ContactPerson, request.ContactPersonEmail,
            request.ContactPersonPhone, request.BillingAddress);

        return entity;
    }

    public static void Update(this Customer entity, CustomerRequest request)
    {
        entity.Update(request.Name, request.Phone, request.Email, request.Website,
            request.ContactPerson, request.ContactPersonEmail, request.ContactPersonPhone,
            request.BillingAddress);
    }
}

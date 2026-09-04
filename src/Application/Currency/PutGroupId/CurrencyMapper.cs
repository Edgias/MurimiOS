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
using EdgIQ.MurimiOS.Domain.Currency;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Currency.PutGroupId;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

public static class CurrencyMapper
{
    public static CurrencyResponse AsApiResponse(this Currency currency)
    {
        CurrencyResponse dto = new()
        {
            Id = currency.Id,
            Name = currency.Name,
            Symbol = currency.Symbol,
            CurrencyCode = currency.CurrencyCode
        };

        return dto;
    }

    public static Currency ToEntity(this CurrencyRequest request)
    {
        Currency currency = new(request.Name, request.CurrencyCode, request.Symbol);

        return currency;
    }

    public static void Update(this Currency currency, CurrencyRequest request)
    {
        currency.Update(request.Name, request.CurrencyCode, request.Symbol);
    }
}

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
using EdgIQ.MurimiOS.Domain.Loan;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Loan.PutGroupId;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;
using Loan = global::EdgIQ.MurimiOS.Domain.Loan.Loan;

public static class LoanMapper
{
    public static LoanResponse AsApiResponse(this Loan entity)
    {
        LoanResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Duration = entity.Duration,
            CurrencyId = entity.CurrencyId,
            CurrencyName = entity.Currency?.Name!,
            EffectiveRate = entity.EffectiveRate,
            InterestRate = entity.InterestRate,
            PrincipalAmount = entity.PrincipalAmount,
        };

        return response;
    }

    public static Loan ToEntity(this LoanRequest request)
    {
        Loan entity = new(request.Name, request.Description, request.Duration,
            request.PrincipalAmount, request.InterestRate, request.EffectiveRate,
            request.CurrencyId);

        return entity;
    }

    public static void Update(this Loan entity, LoanRequest request)
    {
        entity.Update(request.Name, request.Description, request.Duration,
            request.PrincipalAmount, request.InterestRate, request.EffectiveRate);
    }
}

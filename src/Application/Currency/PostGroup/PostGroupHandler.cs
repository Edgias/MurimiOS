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
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Currency.PostGroup;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

public sealed partial class PostGroupHandler(ILegacyDbContext dbContext) : IRequestHandler<PostGroupCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostGroupCommand message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.request);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext, CurrencyRequest request)         {
            Currency currency = request.ToEntity();

            dbContext.Add(currency);
            await dbContext.SaveChangesAsync();

            return UseCaseResults.Created($"/currencies/{currency.Id}", currency.AsApiResponse());
        }

}

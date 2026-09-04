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
using EdgIQ.MurimiOS.Domain.Bins;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Bins.PutGroupId;
using Bin = global::EdgIQ.MurimiOS.Domain.Bins.Bin;


public sealed class PutGroupIdHandler(ILegacyDbContext dbContext) : IRequestHandler<PutGroupIdCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PutGroupIdCommand message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.id, message.request);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext,
            Guid id, BinRequest request)         {
            if (id == Guid.Empty)
            {
                return UseCaseResults.BadRequest();
            }

            Bin? bin = await dbContext.Bins.FindAsync(id);

            if (bin is null)
            {
                return UseCaseResults.NotFound();
            }

            bin.Update(request);

            dbContext.Update(bin);
            await dbContext.SaveChangesAsync();

            return UseCaseResults.Ok();
        }

}

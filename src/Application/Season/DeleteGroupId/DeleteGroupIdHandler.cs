using Microsoft.EntityFrameworkCore;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Season.DeleteGroupId;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;



public sealed partial class DeleteGroupIdHandler(ILegacyDbContext dbContext) : IRequestHandler<DeleteGroupIdCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DeleteGroupIdCommand message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.id);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext, Guid id)         {
            int rowsAffected = await dbContext.Seasons
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? UseCaseResults.NotFound() : UseCaseResults.Ok();
        }

}

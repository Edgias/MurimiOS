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
using EdgIQ.MurimiOS.Domain.SeasonStatus;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SeasonStatus.GetGroupId;
using SeasonStatus = global::EdgIQ.MurimiOS.Domain.SeasonStatus.SeasonStatus;

public sealed partial class GetGroupIdHandler(ILegacyDbContext dbContext) : IRequestHandler<GetGroupIdQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetGroupIdQuery message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.id);
private static async Task<UseCaseResult> ExecuteAsync            (ILegacyDbContext dbContext, Guid id)         {
            return await dbContext.SeasonStatuses.FindAsync(id) switch
            {
                SeasonStatus seasonStatus when seasonStatus is not null => UseCaseResults.Ok(seasonStatus.AsApiResponse()),
                _ => UseCaseResults.NotFound()
            };
        }

}

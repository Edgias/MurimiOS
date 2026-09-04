using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiFarmsFarmIdGuidSeasons;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;

public sealed partial class GetApiFarmsFarmIdGuidSeasonsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiFarmsFarmIdGuidSeasonsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiFarmsFarmIdGuidSeasonsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
            SeasonResponse[] results = await db.GrowingSeasons
                .Where(x => x.FarmId == farmId)
                .OrderByDescending(x => x.StartsOn)
                .Select(x => new SeasonResponse(x.Id, x.FarmId, x.Name, x.StartsOn, x.EndsOn, x.Status))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}

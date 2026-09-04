using EdgIQ.MurimiOS.Domain.GrowingSeason;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.PostApiFarmsFarmIdGuidSeasons;
using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

public sealed partial class PostApiFarmsFarmIdGuidSeasonsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostApiFarmsFarmIdGuidSeasonsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostApiFarmsFarmIdGuidSeasonsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            CreateSeasonRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)
                || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken))
            {
                return UseCaseResults.NotFound();
            }

            Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            var season = new GrowingSeason(id, farmId, request.Name, request.StartsOn, request.EndsOn, request.Status);
            db.GrowingSeasons.Add(season);
            await db.SaveChangesAsync(cancellationToken);

            return UseCaseResults.Created(
                $"/api/v1/farms/{farmId}/seasons/{id}",
                new SeasonResponse(id, farmId, season.Name, season.StartsOn, season.EndsOn, season.Status));
        }

}

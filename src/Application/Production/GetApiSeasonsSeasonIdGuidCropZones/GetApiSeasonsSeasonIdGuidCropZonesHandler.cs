using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiSeasonsSeasonIdGuidCropZones;


public sealed partial class GetApiSeasonsSeasonIdGuidCropZonesHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiSeasonsSeasonIdGuidCropZonesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiSeasonsSeasonIdGuidCropZonesQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.seasonId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid seasonId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            Guid? farmId = await db.GrowingSeasons
                .Where(season => season.Id == seasonId)
                .Select(season => (Guid?)season.FarmId)
                .SingleOrDefaultAsync(cancellationToken);
            if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
                return UseCaseResults.NotFound();
            CropZoneResponse[] results = await db.CropZones
                .Where(x => x.SeasonId == seasonId)
                .OrderBy(x => x.Name)
                .Select(x => new CropZoneResponse(
                    x.Id,
                    x.SeasonId,
                    x.FieldId,
                    x.BoundaryVersionId,
                    x.Name,
                    x.Crop,
                    x.Variety,
                    x.AreaHa))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}

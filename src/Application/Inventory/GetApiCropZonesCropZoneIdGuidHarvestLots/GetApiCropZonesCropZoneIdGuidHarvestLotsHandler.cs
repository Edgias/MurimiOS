using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots;


public sealed partial class GetApiCropZonesCropZoneIdGuidHarvestLotsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiCropZonesCropZoneIdGuidHarvestLotsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiCropZonesCropZoneIdGuidHarvestLotsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.cropZoneId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid cropZoneId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            Guid? farmId = await (
                from zone in db.CropZones
                join season in db.GrowingSeasons on zone.SeasonId equals season.Id
                where zone.Id == cropZoneId
                select (Guid?)season.FarmId).SingleOrDefaultAsync(cancellationToken);
            if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
                return UseCaseResults.NotFound();
            HarvestLotResponse[] results = await db.HarvestLots
                .Where(x => x.CropZoneId == cropZoneId)
                .OrderByDescending(x => x.HarvestedAt)
                .Select(x => new HarvestLotResponse(
                    x.Id,
                    x.CropZoneId,
                    x.LotCode,
                    x.Quantity,
                    x.Unit,
                    x.EnteredQuantity,
                    x.EnteredUnit,
                    x.DisplayUnit,
                    x.HarvestedAt,
                    x.Notes))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}

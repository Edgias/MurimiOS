using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiCropZonesCropZoneIdGuidOperations;


public sealed partial class GetApiCropZonesCropZoneIdGuidOperationsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiCropZonesCropZoneIdGuidOperationsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiCropZonesCropZoneIdGuidOperationsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.cropZoneId, db, scopeAccess, cancellationToken);
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
            FieldOperationResponse[] results = await db.FieldOperations
                .Where(x => x.CropZoneId == cropZoneId && x.SupersededById == null)
                .OrderByDescending(x => x.OccurredAt)
                .Select(x => new FieldOperationResponse(
                    x.Id,
                    x.CropZoneId!.Value,
                    x.OperationType,
                    x.OccurredAt,
                    x.RecordedAt,
                    x.Notes))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}

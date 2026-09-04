using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.CloseRecall;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class CloseRecallHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CloseRecallCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CloseRecallCommand message, CancellationToken cancellationToken) => CloseRecallAsync(message.recallId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CloseRecallAsync(Guid recallId, CloseRecallRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        RecallCase? recall = await db.RecallCases.SingleOrDefaultAsync(x => x.Id == recallId, cancellationToken);
        if (recall is null) return UseCaseResults.NotFound();
        RecallSource[] sources = await db.RecallSources.AsNoTracking().Where(x => x.RecallCaseId == recallId).ToArrayAsync(cancellationToken);
        foreach (RecallSource source in sources)
        {
            Guid? farmId = await ResolveObjectFarmAsync(source.ObjectType, source.ObjectId, db, cancellationToken);
            if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        }
        try { recall.Close(request.ClosedAt); }
        catch (InvalidOperationException ex) { return UseCaseResults.Conflict(new { code = "invalid_recall_status", message = ex.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.NoContent();
    }


    private static async Task<Guid?> ResolveObjectFarmAsync(TraceObjectType type, Guid id, IPlatformDbContext db,
        CancellationToken cancellationToken) => type switch
    {
        TraceObjectType.HarvestLot => await (from lot in db.HarvestLots join zone in db.CropZones on lot.CropZoneId equals zone.Id
            join season in db.GrowingSeasons on zone.SeasonId equals season.Id where lot.Id == id select (Guid?)season.FarmId)
            .SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.StorageLot => await db.StorageLots.Where(x => x.Id == id).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.Delivery => await db.Deliveries.Where(x => x.Id == id).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.LivestockFeedBatch => await db.LivestockFeedBatches.Where(x => x.Id == id)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.InputStockLot => await (from lot in db.InputStockLots join input in db.AgriculturalInputs on lot.InputId equals input.Id
            where lot.Id == id select (Guid?)input.FarmId).SingleOrDefaultAsync(cancellationToken),
        _ => null
    };

}

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
namespace EdgIQ.MurimiOS.Application.Traceability.ListRecalls;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using RecallStatus = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallStatus;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class ListRecallsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListRecallsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRecallsQuery message, CancellationToken cancellationToken) => ListRecallsAsync(db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListRecallsAsync(IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        RecallCase[] recalls = await db.RecallCases.AsNoTracking().OrderByDescending(x => x.InitiatedAt).ToArrayAsync(cancellationToken);
        RecallSource[] sources = await db.RecallSources.AsNoTracking().ToArrayAsync(cancellationToken);
        var visible = new List<RecallResponse>();
        foreach (RecallCase recall in recalls)
        {
            RecallSource[] recallSources = sources.Where(s => s.RecallCaseId == recall.Id).ToArray();
            bool allowed = true;
            foreach (RecallSource source in recallSources)
            {
                Guid? farmId = await ResolveObjectFarmAsync(source.ObjectType, source.ObjectId, db, cancellationToken);
                if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) { allowed = false; break; }
            }
            if (allowed) visible.Add(new RecallResponse(recall.Id, recall.Reference, recall.Reason, recall.Status,
                recall.InitiatedAt, recall.ClosedAt, recallSources.Select(s => new TraceObjectReference(s.ObjectType, s.ObjectId))));
        }
        return UseCaseResults.Ok(visible);
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

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
namespace EdgIQ.MurimiOS.Application.Traceability.CreateRecall;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using RecallStatus = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallStatus;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class CreateRecallHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateRecallCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRecallCommand message, CancellationToken cancellationToken) => CreateRecallAsync(message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateRecallAsync(CreateRecallRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (request.Sources.Count == 0) return Bad("recall_sources_required", "At least one recall source is required.");
        foreach (TraceObjectReference source in request.Sources)
        {
            Guid? farmId = await ResolveObjectFarmAsync(source.ObjectType, source.ObjectId, db, cancellationToken);
            if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        }
        Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
        var recall = new RecallCase(id, request.Reference, request.Reason, request.InitiatedAt);
        db.RecallCases.Add(recall);
        foreach (TraceObjectReference source in request.Sources.Distinct())
            db.RecallSources.Add(new RecallSource(Guid.NewGuid(), id, source.ObjectType, source.ObjectId));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/recalls/{id}", new RecallResponse(id, recall.Reference, recall.Reason,
            recall.Status, recall.InitiatedAt, recall.ClosedAt, request.Sources));
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

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
namespace EdgIQ.MurimiOS.Application.Traceability.GetRecallImpact;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class GetRecallImpactHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetRecallImpactQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetRecallImpactQuery message, CancellationToken cancellationToken) => GetRecallImpactAsync(message.recallId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetRecallImpactAsync(Guid recallId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        RecallCase? recall = await db.RecallCases.AsNoTracking().SingleOrDefaultAsync(x => x.Id == recallId, cancellationToken);
        if (recall is null) return UseCaseResults.NotFound();
        RecallSource[] sources = await db.RecallSources.AsNoTracking().Where(x => x.RecallCaseId == recallId).ToArrayAsync(cancellationToken);
        foreach (RecallSource source in sources)
        {
            Guid? farmId = await ResolveObjectFarmAsync(source.ObjectType, source.ObjectId, db, cancellationToken);
            if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        }
        TraceLink[] links = await db.TraceLinks.AsNoTracking().ToArrayAsync(cancellationToken);
        var nodes = new HashSet<TraceObjectReference>();
        var edges = new Dictionary<Guid, LineageEdge>();
        foreach (RecallSource source in sources)
        {
            LineageGraph graph = BuildGraph(source.ObjectType, source.ObjectId, links, false, 25);
            nodes.UnionWith(graph.Nodes);
            foreach (LineageEdge edge in graph.Edges) edges[edge.LinkId] = edge;
        }
        Guid[] affectedDeliveryIds = nodes.Where(x => x.ObjectType == TraceObjectType.Delivery).Select(x => x.ObjectId).ToArray();
        DeliveryResponse[] deliveries = await db.Deliveries.AsNoTracking().Where(x => affectedDeliveryIds.Contains(x.Id))
            .Select(x => new DeliveryResponse(x.Id, x.FarmId, x.DeliveryNumber, x.Recipient, x.Destination,
                x.Status, x.DispatchedAt, x.ReceivedAt, x.TransactionReference)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(new RecallImpactResponse(recallId, nodes, edges.Values, deliveries));
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


    private static LineageGraph BuildGraph(TraceObjectType rootType, Guid rootId, IReadOnlyList<TraceLink> links,
        bool backward, int depthLimit)
    {
        var root = new TraceObjectReference(rootType, rootId);
        var nodes = new HashSet<TraceObjectReference> { root };
        var edges = new Dictionary<Guid, LineageEdge>();
        var frontier = new HashSet<TraceObjectReference> { root };
        for (int depth = 0; depth < depthLimit && frontier.Count > 0; depth++)
        {
            var next = new HashSet<TraceObjectReference>();
            foreach (TraceLink link in links)
            {
                var from = new TraceObjectReference(link.FromType, link.FromId);
                var to = new TraceObjectReference(link.ToType, link.ToId);
                bool matches = backward ? frontier.Contains(to) : frontier.Contains(from);
                if (!matches) continue;
                edges[link.Id] = new LineageEdge(link.Id, link.EventId, from, to, link.Relationship);
                TraceObjectReference candidate = backward ? from : to;
                if (nodes.Add(candidate)) next.Add(candidate);
            }
            frontier = next;
        }
        return new LineageGraph(root, backward ? "backward" : "forward", nodes, edges.Values);
    }

}

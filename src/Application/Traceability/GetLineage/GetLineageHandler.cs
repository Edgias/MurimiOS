using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.GetLineage;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class GetLineageHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetLineageQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetLineageQuery message, CancellationToken cancellationToken) => GetLineageAsync(message.objectType, message.objectId, message.direction, message.maxDepth, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetLineageAsync(string objectType, Guid objectId, string? direction, int? maxDepth,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!TryObjectType(objectType, out TraceObjectType parsed)) return Bad("unsupported_object_type", "Unknown trace object type.");
        Guid? farmId = await ResolveObjectFarmAsync(parsed, objectId, db, cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        bool backward = string.Equals(direction, "backward", StringComparison.OrdinalIgnoreCase);
        int depthLimit = Math.Clamp(maxDepth ?? 10, 1, 25);
        TraceLink[] links = await db.TraceLinks.AsNoTracking().ToArrayAsync(cancellationToken);
        LineageGraph graph = BuildGraph(parsed, objectId, links, backward, depthLimit);
        return UseCaseResults.Ok(graph);
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


    private static bool TryObjectType(string value, out TraceObjectType type) =>
        Enum.TryParse(value.Replace("-", string.Empty), true, out type) && Enum.IsDefined(type);

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.CreateDelivery;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;
using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using DeliveryLine = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryLine;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceabilityMassBalance = global::EdgIQ.MurimiOS.Domain.StorageLot.TraceabilityMassBalance;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class CreateDeliveryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateDeliveryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateDeliveryCommand message, CancellationToken cancellationToken) => CreateDeliveryAsync(message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateDeliveryAsync(CreateDeliveryRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (request.Lines.Count == 0) return Bad("delivery_lines_required", "At least one delivery line is required.");
        if (request.Lines.Select(x => (x.ObjectType, x.ObjectId)).Distinct().Count() != request.Lines.Count)
            return Bad("duplicate_source", "Each source lot may appear only once.");

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            await AcquireLotLocksAsync(request.Lines.Select(x => (x.ObjectType, x.ObjectId)), db, cancellationToken);
            var resolved = new List<(DeliverySourceLine Line, SourceLot Source)>();
            foreach (DeliverySourceLine line in request.Lines)
            {
                SourceLot? source = await LoadCropSourceAsync(line.ObjectType, line.ObjectId, db, cancellationToken);
                if (source is null || !await scope.CanAccessFarmAsync(source.FarmId, cancellationToken)) return UseCaseResults.NotFound();
                MeasuredQuantity quantity = MeasurementUnits.Capture(line.Quantity, line.Unit, line.Unit);
                if (!quantity.CanonicalUnit.Equals(source.Unit, StringComparison.OrdinalIgnoreCase))
                    return Bad("unit_mismatch", $"Source {line.ObjectId} is measured in {source.Unit}.");
                if (quantity.CanonicalValue > source.Available)
                    return UseCaseResults.Conflict(new { code = "insufficient_lot_balance", objectId = line.ObjectId, available = source.Available, unit = source.Unit });
                resolved.Add((line, source));
            }
            if (resolved.Select(x => x.Source.FarmId).Distinct().Count() != 1)
                return Bad("multiple_farms", "A delivery must be dispatched from one farm.");

            Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            Guid farmId = resolved[0].Source.FarmId;
            var delivery = new Delivery(id, farmId, request.DeliveryNumber, request.Recipient, request.Destination,
                request.DispatchedAt, request.TransactionReference);
            var traceEvent = new TraceEvent(Guid.NewGuid(), farmId, TraceEventType.Transaction, "shipping", "in_transit",
                request.DispatchedAt, "destination", null, "DeliveryDispatch", id, request.TransactionReference);
            db.Deliveries.Add(delivery);
            db.TraceEvents.Add(traceEvent);
            db.TraceEventObjects.Add(new TraceEventObject(Guid.NewGuid(), traceEvent.Id, TraceObjectType.Delivery, id, TraceObjectRole.Output));
            foreach ((DeliverySourceLine line, _) in resolved)
            {
                db.DeliveryLines.Add(new DeliveryLine(Guid.NewGuid(), id, line.ObjectType, line.ObjectId, line.Quantity, line.Unit));
                db.TraceEventObjects.Add(new TraceEventObject(Guid.NewGuid(), traceEvent.Id, line.ObjectType, line.ObjectId,
                    TraceObjectRole.Input, line.Quantity, line.Unit));
                db.TraceLinks.Add(new TraceLink(Guid.NewGuid(), traceEvent.Id, line.ObjectType, line.ObjectId,
                    TraceObjectType.Delivery, id, TraceRelationshipType.DeliveredAs));
            }
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/deliveries/{id}", new DeliveryResponse(id, farmId,
                delivery.DeliveryNumber, delivery.Recipient, delivery.Destination, delivery.Status,
                delivery.DispatchedAt, delivery.ReceivedAt, delivery.TransactionReference));
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return UseCaseResults.Conflict(new { code = "delivery_conflict", message = "The lot balance or delivery changed; retry with current data." });
        }
    }


    private static async Task<SourceLot?> LoadCropSourceAsync(TraceObjectType type, Guid id, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        SourceBase? item = type switch
        {
            TraceObjectType.HarvestLot => await (from lot in db.HarvestLots
                join zone in db.CropZones on lot.CropZoneId equals zone.Id
                join season in db.GrowingSeasons on zone.SeasonId equals season.Id
                where lot.Id == id select new SourceBase(season.FarmId, zone.Crop, zone.Variety, lot.Quantity, lot.Unit))
                .SingleOrDefaultAsync(cancellationToken),
            TraceObjectType.StorageLot => await db.StorageLots.Where(x => x.Id == id)
                .Select(x => new SourceBase(x.FarmId, x.Crop, x.Variety, x.Quantity, x.Unit)).SingleOrDefaultAsync(cancellationToken),
            _ => null
        };
        if (item is null) return null;
        decimal consumed = await db.TraceEventObjects
            .Where(x => x.ObjectType == type && x.ObjectId == id && x.Role == TraceObjectRole.Input)
            .SumAsync(x => x.Quantity ?? 0, cancellationToken);
        return new SourceLot(item.FarmId, item.Crop, item.Variety, item.Original, item.Unit, consumed,
            TraceabilityMassBalance.Available(item.Original, consumed));
    }


    private static async Task AcquireLotLocksAsync(IEnumerable<(TraceObjectType Type, Guid Id)> lots,
        IPlatformDbContext db, CancellationToken cancellationToken)
    {
        foreach ((TraceObjectType type, Guid id) in lots.OrderBy(x => x.Type).ThenBy(x => x.Id))
        {
            string key = $"crop-lot:{type}:{id:N}";
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0));", cancellationToken);
        }
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });


    private sealed record SourceBase(Guid FarmId, string Crop, string? Variety, decimal Original, string Unit);

    private sealed record SourceLot(Guid FarmId, string Crop, string? Variety, decimal Original, string Unit, decimal Consumed, decimal Available);

}

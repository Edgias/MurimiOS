using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.CreateTransformation;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;
using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceabilityMassBalance = global::EdgIQ.MurimiOS.Domain.StorageLot.TraceabilityMassBalance;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class CreateTransformationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateTransformationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateTransformationCommand message, CancellationToken cancellationToken) => CreateTransformationAsync(message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateTransformationAsync(CreateTransformationRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (request.Inputs.Count == 0 || request.Outputs.Count == 0)
            return Bad("transformation_lines_required", "At least one input and output are required.");
        if (request.Inputs.Select(x => (x.ObjectType, x.ObjectId)).Distinct().Count() != request.Inputs.Count)
            return Bad("duplicate_input", "Each source lot may appear only once.");

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            await AcquireLotLocksAsync(request.Inputs.Select(x => (x.ObjectType, x.ObjectId)), db, cancellationToken);
            SourceLot[] sources = new SourceLot[request.Inputs.Count];
            MeasuredQuantity[] inputQuantities = new MeasuredQuantity[request.Inputs.Count];
            for (int i = 0; i < request.Inputs.Count; i++)
            {
                TransformationInput line = request.Inputs[i];
                SourceLot? source = await LoadCropSourceAsync(line.ObjectType, line.ObjectId, db, cancellationToken);
                if (source is null || !await scope.CanAccessFarmAsync(source.FarmId, cancellationToken)) return UseCaseResults.NotFound();
                MeasuredQuantity measured = MeasurementUnits.Capture(line.Quantity, line.Unit, line.Unit);
                if (!measured.CanonicalUnit.Equals(source.Unit, StringComparison.OrdinalIgnoreCase))
                    return Bad("unit_mismatch", $"Input {line.ObjectId} is measured in {source.Unit}.");
                if (measured.CanonicalValue > source.Available)
                    return UseCaseResults.Conflict(new { code = "insufficient_lot_balance", objectId = line.ObjectId, available = source.Available, unit = source.Unit });
                sources[i] = source;
                inputQuantities[i] = measured;
            }

            if (sources.Select(x => x.FarmId).Distinct().Count() != 1 || sources.Select(x => x.Crop).Distinct(StringComparer.OrdinalIgnoreCase).Count() != 1)
                return Bad("incompatible_inputs", "Transformation inputs must belong to one farm and crop.");
            string canonicalUnit = inputQuantities[0].CanonicalUnit;
            if (inputQuantities.Any(x => !x.CanonicalUnit.Equals(canonicalUnit, StringComparison.OrdinalIgnoreCase)))
                return Bad("incompatible_units", "All transformation lines must share a canonical unit.");

            MeasuredQuantity[] outputQuantities = request.Outputs
                .Select(x => MeasurementUnits.Capture(x.Quantity, x.Unit, x.Unit)).ToArray();
            if (outputQuantities.Any(x => !x.CanonicalUnit.Equals(canonicalUnit, StringComparison.OrdinalIgnoreCase)))
                return Bad("incompatible_units", "Inputs, outputs, and loss must share a canonical unit.");
            decimal loss = request.DeclaredLoss <= 0 ? 0 : MeasurementUnits.Capture(request.DeclaredLoss, request.LossUnit ?? canonicalUnit, request.LossUnit).CanonicalValue;
            TraceabilityMassBalance.EnsureBalanced(inputQuantities.Sum(x => x.CanonicalValue), outputQuantities.Sum(x => x.CanonicalValue), loss);

            Guid transformationId = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            Guid farmId = sources[0].FarmId;
            string crop = sources[0].Crop;
            string? variety = sources.Select(x => x.Variety).Distinct(StringComparer.OrdinalIgnoreCase).Count() == 1 ? sources[0].Variety : null;
            var traceEvent = new TraceEvent(transformationId, farmId,
                request.Inputs.Count == 1 && request.Outputs.Count > 1 ? TraceEventType.Aggregation : TraceEventType.Transformation,
                request.BusinessStep ?? "packing", request.Disposition ?? "active", request.OccurredAt,
                "facility", request.LocationId, "Transformation", transformationId, request.TransactionReference,
                JsonSerializer.Serialize(new { request.DeclaredLoss, request.LossUnit, request.Notes }));
            db.TraceEvents.Add(traceEvent);

            for (int i = 0; i < request.Inputs.Count; i++)
            {
                TransformationInput line = request.Inputs[i];
                db.TraceEventObjects.Add(new TraceEventObject(Guid.NewGuid(), transformationId, line.ObjectType,
                    line.ObjectId, TraceObjectRole.Input, line.Quantity, line.Unit));
            }

            var outputResponses = new List<StorageLotResponse>();
            for (int i = 0; i < request.Outputs.Count; i++)
            {
                TransformationOutput line = request.Outputs[i];
                Guid outputId = line.Id is { } id && id != Guid.Empty ? id : Guid.NewGuid();
                var lot = new StorageLot(outputId, farmId, line.LotCode, crop, variety, line.Quantity,
                    line.Unit, request.OccurredAt, line.StorageLocation);
                db.StorageLots.Add(lot);
                db.TraceEventObjects.Add(new TraceEventObject(Guid.NewGuid(), transformationId,
                    TraceObjectType.StorageLot, outputId, TraceObjectRole.Output, line.Quantity, line.Unit));
                foreach (TransformationInput input in request.Inputs)
                    db.TraceLinks.Add(new TraceLink(Guid.NewGuid(), transformationId, input.ObjectType, input.ObjectId,
                        TraceObjectType.StorageLot, outputId,
                        request.Inputs.Count == 1 && request.Outputs.Count > 1 ? TraceRelationshipType.AggregatedInto : TraceRelationshipType.TransformedInto));
                outputResponses.Add(new StorageLotResponse(outputId, farmId, line.LotCode, crop, variety,
                    lot.Quantity, lot.Unit, lot.EnteredQuantity, lot.EnteredUnit, lot.PackedAt, line.StorageLocation));
            }

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/traceability/events/{transformationId}",
                new TransformationResponse(transformationId, loss, canonicalUnit, outputResponses));
        }
        catch (InvalidOperationException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Bad("mass_balance_failed", ex.Message);
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return UseCaseResults.Conflict(new { code = "traceability_conflict", message = "The lot balance or trace record changed; retry with current data." });
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

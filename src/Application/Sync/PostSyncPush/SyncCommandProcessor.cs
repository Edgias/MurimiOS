using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Sync.PostSyncPush;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;
using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using SyncCommandLog = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandLog;
using SyncCommandStatus = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResultType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResultType;

public sealed class SyncCommandProcessor(
    IPlatformDbContext dbContext,
    IOrganisationScopeAccess scopeAccess,
    IRequestActorContext actorContext,
    ITenantContext tenantContext,
    TimeProvider timeProvider)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<SyncProcessingOutcome> PushAsync(SyncPushRequest request, CancellationToken cancellationToken)
    {
        if (request.DeviceId == Guid.Empty)
        {
            return SyncProcessingOutcome.Invalid("device_required", "A device id is required.");
        }

        if (request.Commands.Count == 0)
        {
            long next = await ExpectedSequenceAsync(request.DeviceId, cancellationToken);
            return SyncProcessingOutcome.Success(new SyncPushResponse(next, []));
        }

        if (request.Commands.Count > 100)
        {
            return SyncProcessingOutcome.Invalid("batch_too_large", "A sync batch may contain at most 100 commands.");
        }

        if (request.Commands.Any(x => x.CommandId == Guid.Empty || x.Sequence <= 0 || string.IsNullOrWhiteSpace(x.Type)))
        {
            return SyncProcessingOutcome.Invalid("invalid_envelope", "Every command requires an id, positive sequence, and type.");
        }

        if (request.Commands.Select(x => x.CommandId).Distinct().Count() != request.Commands.Count)
        {
            return SyncProcessingOutcome.Invalid("duplicate_batch_id", "A command id may occur only once in a batch.");
        }

        Guid[] commandIds = request.Commands.Select(x => x.CommandId).ToArray();
        Dictionary<Guid, SyncCommandLog> existing = await dbContext.SyncCommands
            .Where(x => commandIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        long expected = await ExpectedSequenceAsync(request.DeviceId, cancellationToken);
        foreach (SyncCommandEnvelope command in request.Commands)
        {
            if (existing.TryGetValue(command.CommandId, out SyncCommandLog? duplicate))
            {
                if (duplicate.DeviceId != request.DeviceId || duplicate.Sequence != command.Sequence)
                {
                    return SyncProcessingOutcome.Invalid(
                        "idempotency_mismatch",
                        "A replayed command id must retain its original device and sequence.");
                }

                continue;
            }

            if (command.Sequence != expected)
            {
                return SyncProcessingOutcome.WithGap(new SyncGapResponse(expected, command.CommandId, command.Sequence));
            }

            expected++;
        }

        var results = new List<SyncCommandResult>(request.Commands.Count);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (SyncCommandEnvelope command in request.Commands)
        {
            if (existing.TryGetValue(command.CommandId, out SyncCommandLog? duplicate))
            {
                results.Add(ToDuplicateResult(duplicate));
                continue;
            }

            DateTimeOffset processedAt = timeProvider.GetUtcNow();
            const string commandSavepoint = "sync_command";
            await transaction.CreateSavepointAsync(commandSavepoint, cancellationToken);
            var log = new SyncCommandLog(
                command.CommandId,
                request.DeviceId,
                command.Sequence,
                command.Type,
                command.SchemaVersion,
                command.CapturedAt,
                processedAt);

            try
            {
                Guid resultId = await ApplyAsync(command, processedAt, cancellationToken);
                log.MarkApplied(resultId);
                results.Add(new SyncCommandResult(command.CommandId, command.Sequence, "Applied", resultId, null, null));
            }
            catch (CommandRejectedException exception)
            {
                await RollbackCommandAsync(transaction, commandSavepoint, cancellationToken);
                log.MarkRejected(exception.Code, exception.Message);
                results.Add(new SyncCommandResult(
                    command.CommandId,
                    command.Sequence,
                    "Rejected",
                    null,
                    exception.Code,
                    exception.Message));
            }
            catch (JsonException exception)
            {
                await RollbackCommandAsync(transaction, commandSavepoint, cancellationToken);
                log.MarkRejected("invalid_payload", exception.Message);
                results.Add(new SyncCommandResult(
                    command.CommandId,
                    command.Sequence,
                    "Rejected",
                    null,
                    "invalid_payload",
                    exception.Message));
            }
            catch (ArgumentException exception)
            {
                await RollbackCommandAsync(transaction, commandSavepoint, cancellationToken);
                log.MarkRejected("validation_failed", exception.Message);
                results.Add(new SyncCommandResult(
                    command.CommandId,
                    command.Sequence,
                    "Rejected",
                    null,
                    "validation_failed",
                    exception.Message));
            }

            dbContext.SyncCommands.Add(log);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.ReleaseSavepointAsync(commandSavepoint, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return SyncProcessingOutcome.Success(new SyncPushResponse(expected, results));
    }

    private async Task<Guid> ApplyAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        if (command.SchemaVersion != 1)
        {
            throw new CommandRejectedException("unsupported_schema", $"Schema version {command.SchemaVersion} is not supported.");
        }

        return command.Type switch
        {
            "RecordFieldOperation" => await RecordFieldOperationAsync(command, processedAt, cancellationToken),
            "RecordCropZoneOperation" => await RecordCropZoneOperationAsync(command, processedAt, cancellationToken),
            "ApplyInputToCropZone" => await ApplyInputToCropZoneAsync(command, processedAt, cancellationToken),
            "RecordHarvestLot" => await RecordHarvestLotAsync(command, processedAt, cancellationToken),
            "RecordLivestockMeasurement" => await RecordLivestockMeasurementAsync(command, processedAt, cancellationToken),
            "RecordLivestockTreatment" => await RecordLivestockTreatmentAsync(command, processedAt, cancellationToken),
            "RecordLivestockMovement" => await RecordLivestockMovementAsync(command, processedAt, cancellationToken),
            "RecordLivestockHealthEvent" => await RecordLivestockHealthEventAsync(command, processedAt, cancellationToken),
            "AllocateLivestockFeed" => await AllocateLivestockFeedAsync(command, processedAt, cancellationToken),
            "StartWorkOrder" => await StartWorkOrderAsync(command, processedAt, cancellationToken),
            "CompleteWorkOrder" => await CompleteWorkOrderAsync(command, processedAt, cancellationToken),
            _ => throw new CommandRejectedException("unknown_command", $"Command type '{command.Type}' is not supported.")
        };
    }

    private async Task<Guid> StartWorkOrderAsync(
        SyncCommandEnvelope command, DateTimeOffset processedAt, CancellationToken cancellationToken)
    {
        StartWorkOrderPayload payload = command.Payload.Deserialize<StartWorkOrderPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A work-order start payload is required.");
        WorkOrder? order = await dbContext.WorkOrders.SingleOrDefaultAsync(x => x.Id == payload.WorkOrderId, cancellationToken);
        if (order is null || !await scopeAccess.CanAccessFarmAsync(order.FarmId, cancellationToken))
            throw new CommandRejectedException("work_order_not_found", "The work order is unavailable in this tenant or farm scope.");
        await EnsureAssignedOperatorAsync(order, cancellationToken);
        WorkOrderStatus previous = order.Status;
        try { order.Start(payload.StartedAt); }
        catch (InvalidOperationException ex) { throw new CommandRejectedException("invalid_work_order_transition", ex.Message); }
        dbContext.WorkOrderStatusHistory.Add(new WorkOrderStatusHistory(Guid.NewGuid(), order.Id, previous,
            order.Status, payload.StartedAt, actorContext.Subject, "Started from an offline device command."));
        AddChangeAndOutbox(order.FarmId, nameof(WorkOrder), order.Id, new
        {
            order.Id, order.FarmId, order.Number, order.Status, order.StartedAt,
            SourceCommandId = command.CommandId
        }, processedAt, "WorkOrder.Started");
        return order.Id;
    }

    private async Task<Guid> CompleteWorkOrderAsync(
        SyncCommandEnvelope command, DateTimeOffset processedAt, CancellationToken cancellationToken)
    {
        CompleteWorkOrderPayload payload = command.Payload.Deserialize<CompleteWorkOrderPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A work-order completion payload is required.");
        WorkOrder? order = await dbContext.WorkOrders.SingleOrDefaultAsync(x => x.Id == payload.WorkOrderId, cancellationToken);
        if (order is null || !await scopeAccess.CanAccessFarmAsync(order.FarmId, cancellationToken))
            throw new CommandRejectedException("work_order_not_found", "The work order is unavailable in this tenant or farm scope.");
        await EnsureAssignedOperatorAsync(order, cancellationToken);
        if (order.Status != WorkOrderStatus.InProgress)
            throw new CommandRejectedException("invalid_work_order_transition", "Only in-progress work can be completed.");

        WorkOrderResource[] resources = await dbContext.WorkOrderResources
            .Where(x => x.WorkOrderId == order.Id && x.Status == WorkResourceStatus.Reserved)
            .ToArrayAsync(cancellationToken);
        foreach (WorkOrderResource resource in resources)
        {
            resource.Consume();
            if (resource.StockLotId is not { } lotId) continue;
            string lockKey = $"input-stock-lot:{lotId:N}";
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
            InputStockLot? lot = await dbContext.InputStockLots.SingleOrDefaultAsync(x => x.Id == lotId, cancellationToken);
            if (lot is null || resource.Quantity > lot.QuantityOnHand)
                throw new CommandRejectedException("insufficient_input_stock", "A reserved input no longer has sufficient stock on hand.");
            lot.Issue(resource.Quantity);
            Guid movementId = Guid.NewGuid();
            dbContext.InventoryMovements.Add(new InventoryMovement(movementId, lotId, InventoryMovementType.Issue,
                resource.Quantity, resource.Unit, payload.CompletedAt, null, command.CommandId,
                $"Consumed by work order {order.Number}."));
            dbContext.WorkOrderResults.Add(new WorkOrderResult(Guid.NewGuid(), order.Id,
                WorkResultType.InventoryMovement, movementId, "Inventory consumed from a reserved work-order resource.",
                processedAt, actorContext.Subject));
        }
        foreach (SyncWorkOrderResultPayload result in payload.Results ?? [])
        {
            if (result.ResultId == Guid.Empty)
                throw new CommandRejectedException("work_result_invalid", "Result identifiers must be non-empty.");
            dbContext.WorkOrderResults.Add(new WorkOrderResult(Guid.NewGuid(), order.Id, result.ResultType,
                result.ResultId, result.Notes, processedAt, actorContext.Subject));
        }
        WorkOrderStatus previous = order.Status;
        order.Complete(payload.CompletedAt, payload.Notes);
        dbContext.WorkOrderStatusHistory.Add(new WorkOrderStatusHistory(Guid.NewGuid(), order.Id, previous,
            order.Status, payload.CompletedAt, actorContext.Subject, payload.Notes));
        AddChangeAndOutbox(order.FarmId, nameof(WorkOrder), order.Id, new
        {
            order.Id, order.FarmId, order.Number, order.Status, order.CompletedAt, order.CompletionNotes,
            ResourcesConsumed = resources.Length, ResultCount = payload.Results?.Count ?? 0,
            SourceCommandId = command.CommandId
        }, processedAt, "WorkOrder.Completed");
        return order.Id;
    }

    private async Task EnsureAssignedOperatorAsync(WorkOrder order, CancellationToken cancellationToken)
    {
        if (actorContext.HasPermission(PlatformPermissions.OperationsAssign)) return;
        if (actorContext.MembershipId is not { } membershipId
            || !await dbContext.WorkOrderAssignments.AnyAsync(x => x.WorkOrderId == order.Id
                && x.MembershipId == membershipId && x.UnassignedAt == null, cancellationToken))
            throw new CommandRejectedException("work_order_not_assigned", "The current operator is not actively assigned to this work order.");
    }

    private async Task<Guid> RecordFieldOperationAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        RecordFieldOperationPayload payload = command.Payload.Deserialize<RecordFieldOperationPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A field operation payload is required.");

        var field = await dbContext.FarmFields
            .Where(x => x.Id == payload.FieldId)
            .Select(x => new { x.Id, x.FarmId })
            .SingleOrDefaultAsync(cancellationToken);
        bool boundaryExists = await dbContext.FieldBoundaryVersions
            .AnyAsync(x => x.Id == payload.BoundaryVersionId && x.FieldId == payload.FieldId, cancellationToken);

        if (field is null || !boundaryExists
            || !await scopeAccess.CanAccessFarmAsync(field.FarmId, cancellationToken))
        {
            throw new CommandRejectedException("field_not_found", "The field or its boundary version is unavailable in this tenant.");
        }

        Guid resultId = Guid.NewGuid();
        var record = new FieldOperationRecord(
            resultId,
            payload.FieldId,
            payload.BoundaryVersionId,
            payload.OperationType,
            payload.OccurredAt,
            processedAt,
            command.CommandId,
            payload.Notes);

        dbContext.FieldOperations.Add(record);
        AddChangeAndOutbox(
            field.FarmId,
            "FieldOperation",
            resultId,
            new
            {
                record.Id,
                record.FieldId,
                record.BoundaryVersionId,
                record.CropZoneId,
                record.OperationType,
                record.OccurredAt,
                record.RecordedAt,
                record.Notes
            },
            processedAt);

        return resultId;
    }

    private async Task<Guid> ApplyInputToCropZoneAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        ApplyInputToCropZonePayload payload = command.Payload.Deserialize<ApplyInputToCropZonePayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "An input-application payload is required.");

        var production = await (
            from zone in dbContext.CropZones
            join season in dbContext.GrowingSeasons on zone.SeasonId equals season.Id
            where zone.Id == payload.CropZoneId
            select new { Zone = zone, season.FarmId })
            .SingleOrDefaultAsync(cancellationToken);
        var stock = await (
            from lot in dbContext.InputStockLots
            join input in dbContext.AgriculturalInputs on lot.InputId equals input.Id
            where lot.Id == payload.StockLotId
            select new { Lot = lot, input.FarmId, input.Name, input.Unit, input.DisplayUnit })
            .SingleOrDefaultAsync(cancellationToken);

        if (production is null || stock is null || production.FarmId != stock.FarmId)
        {
            throw new CommandRejectedException(
                "input_context_invalid",
                "The crop zone and stock lot must belong to the same tenant and farm.");
        }
        if (!await scopeAccess.CanAccessFarmAsync(production.FarmId, cancellationToken))
            throw new CommandRejectedException("production_context_invalid", "The crop zone is unavailable in this organisation scope.");

        if (payload.Quantity <= 0)
        {
            throw new CommandRejectedException("invalid_quantity", "Applied quantity must be positive.");
        }

        MeasuredQuantity appliedQuantity;
        try
        {
            appliedQuantity = MeasurementUnits.Capture(payload.Quantity, stock.DisplayUnit, stock.DisplayUnit);
        }
        catch (ArgumentException exception)
        {
            throw new CommandRejectedException("invalid_quantity_unit", exception.Message);
        }

        if (appliedQuantity.CanonicalValue > stock.Lot.QuantityOnHand)
        {
            throw new CommandRejectedException("insufficient_stock", "The issue quantity exceeds stock on hand.");
        }

        DateOnly operationDate = DateOnly.FromDateTime(payload.OccurredAt.UtcDateTime);
        if (stock.Lot.ExpiresOn is { } expiresOn && operationDate > expiresOn)
        {
            throw new CommandRejectedException("stock_expired", "Expired stock cannot be applied to a crop zone.");
        }

        Guid operationId = Guid.NewGuid();
        var operation = new FieldOperationRecord(
            operationId,
            production.Zone.FieldId,
            production.Zone.BoundaryVersionId,
            "InputApplication",
            payload.OccurredAt,
            processedAt,
            command.CommandId,
            payload.Notes,
            production.Zone.Id);
        var movement = new InventoryMovement(
            Guid.NewGuid(),
            stock.Lot.Id,
            InventoryMovementType.Issue,
            payload.Quantity,
            stock.DisplayUnit,
            payload.OccurredAt,
            operationId,
            command.CommandId,
            payload.Notes);
        stock.Lot.Issue(movement.Quantity);

        dbContext.FieldOperations.Add(operation);
        dbContext.InventoryMovements.Add(movement);
        AddChangeAndOutbox(
            production.FarmId,
            "InputApplication",
            operationId,
            new
            {
                OperationId = operationId,
                MovementId = movement.Id,
                CropZoneId = production.Zone.Id,
                StockLotId = stock.Lot.Id,
                Input = stock.Name,
                Quantity = movement.Quantity,
                movement.Unit,
                movement.EnteredQuantity,
                movement.EnteredUnit,
                movement.DisplayUnit,
                QuantityOnHand = stock.Lot.QuantityOnHand,
                payload.OccurredAt,
                payload.Notes
            },
            processedAt);

        return operationId;
    }

    private async Task<Guid> RecordHarvestLotAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        RecordHarvestLotPayload payload = command.Payload.Deserialize<RecordHarvestLotPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A harvest-lot payload is required.");

        Guid? farmId = await (
            from zone in dbContext.CropZones
            join season in dbContext.GrowingSeasons on zone.SeasonId equals season.Id
            where zone.Id == payload.CropZoneId
            select (Guid?)season.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
        {
            throw new CommandRejectedException("crop_zone_not_found", "The crop zone is unavailable in this tenant.");
        }

        Guid harvestLotId = Guid.NewGuid();
        var harvestLot = new HarvestLot(
            harvestLotId,
            payload.CropZoneId,
            payload.LotCode,
            payload.Quantity,
            payload.Unit,
            payload.HarvestedAt,
            command.CommandId,
            payload.Notes);
        dbContext.HarvestLots.Add(harvestLot);
        Guid traceEventId = Guid.NewGuid();
        var traceEvent = new TraceEvent(
            traceEventId,
            farmId.Value,
            TraceEventType.Object,
            "harvesting",
            "harvested",
            payload.HarvestedAt,
            "crop_zone",
            payload.CropZoneId,
            "HarvestLot",
            harvestLotId,
            payload.LotCode,
            JsonSerializer.Serialize(new { payload.Notes }));
        dbContext.TraceEvents.Add(traceEvent);
        dbContext.TraceEventObjects.Add(new TraceEventObject(
            Guid.NewGuid(), traceEventId, TraceObjectType.HarvestLot, harvestLotId,
            TraceObjectRole.Output, payload.Quantity, payload.Unit));

        var appliedInputs = await (
            from movement in dbContext.InventoryMovements
            join operation in dbContext.FieldOperations on movement.OperationId equals operation.Id
            where operation.CropZoneId == payload.CropZoneId
                && operation.SupersededById == null
                && movement.Type == InventoryMovementType.Issue
            group movement by new { movement.StockLotId, movement.Unit } into applied
            select new { applied.Key.StockLotId, applied.Key.Unit, Quantity = applied.Sum(x => x.Quantity) })
            .ToArrayAsync(cancellationToken);
        foreach (var applied in appliedInputs)
        {
            dbContext.TraceEventObjects.Add(new TraceEventObject(
                Guid.NewGuid(), traceEventId, TraceObjectType.InputStockLot, applied.StockLotId,
                TraceObjectRole.Input, applied.Quantity, applied.Unit));
            dbContext.TraceLinks.Add(new TraceLink(
                Guid.NewGuid(), traceEventId, TraceObjectType.InputStockLot, applied.StockLotId,
                TraceObjectType.HarvestLot, harvestLotId, TraceRelationshipType.InputApplied));
        }
        AddChangeAndOutbox(
            farmId.Value,
            "HarvestLot",
            harvestLotId,
            new
            {
                harvestLot.Id,
                harvestLot.CropZoneId,
                harvestLot.LotCode,
                harvestLot.Quantity,
                harvestLot.Unit,
                harvestLot.EnteredQuantity,
                harvestLot.EnteredUnit,
                harvestLot.DisplayUnit,
                harvestLot.HarvestedAt,
                harvestLot.Notes
            },
            processedAt);

        return harvestLotId;
    }

    private async Task<Guid> RecordCropZoneOperationAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        RecordCropZoneOperationPayload payload = command.Payload.Deserialize<RecordCropZoneOperationPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A crop-zone operation payload is required.");

        var productionContext = await (
            from zone in dbContext.CropZones
            join season in dbContext.GrowingSeasons on zone.SeasonId equals season.Id
            where zone.Id == payload.CropZoneId
            select new { Zone = zone, season.FarmId }).SingleOrDefaultAsync(cancellationToken);
        if (productionContext is null
            || !await scopeAccess.CanAccessFarmAsync(productionContext.FarmId, cancellationToken))
        {
            throw new CommandRejectedException("crop_zone_not_found", "The crop zone is unavailable in this tenant.");
        }

        Guid resultId = Guid.NewGuid();
        var record = new FieldOperationRecord(
            resultId,
            productionContext.Zone.FieldId,
            productionContext.Zone.BoundaryVersionId,
            payload.OperationType,
            payload.OccurredAt,
            processedAt,
            command.CommandId,
            payload.Notes,
            productionContext.Zone.Id);

        dbContext.FieldOperations.Add(record);
        AddChangeAndOutbox(
            productionContext.FarmId,
            "CropZoneOperation",
            resultId,
            new
            {
                record.Id,
                record.CropZoneId,
                record.FieldId,
                record.BoundaryVersionId,
                record.OperationType,
                record.OccurredAt,
                record.RecordedAt,
                record.Notes
            },
            processedAt);

        return resultId;
    }

    private async Task<Guid> RecordLivestockMeasurementAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        RecordLivestockMeasurementPayload payload = command.Payload.Deserialize<RecordLivestockMeasurementPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A livestock measurement payload is required.");

        Guid? farmId = await dbContext.LivestockSubjects
            .Where(x => x.Id == payload.SubjectId)
            .Select(x => (Guid?)x.FarmId)
            .SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
        {
            throw new CommandRejectedException("subject_not_found", "The livestock subject is unavailable in this tenant.");
        }

        Guid resultId = Guid.NewGuid();
        var record = new LivestockMeasurementRecord(
            resultId,
            payload.SubjectId,
            payload.Metric,
            payload.Value,
            payload.Unit,
            payload.OccurredAt,
            processedAt,
            command.CommandId);

        dbContext.LivestockMeasurements.Add(record);
        AddChangeAndOutbox(
            farmId.Value,
            "LivestockMeasurement",
            resultId,
            new
            {
                record.Id,
                record.SubjectId,
                record.Metric,
                record.Value,
                record.Unit,
                record.EnteredValue,
                record.EnteredUnit,
                record.DisplayUnit,
                record.OccurredAt,
                record.RecordedAt
            },
            processedAt);

        return resultId;
    }

    private async Task<Guid> RecordLivestockTreatmentAsync(
        SyncCommandEnvelope command,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        RecordLivestockTreatmentPayload payload = command.Payload.Deserialize<RecordLivestockTreatmentPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A livestock-treatment payload is required.");

        var subject = await dbContext.LivestockSubjects
            .Where(x => x.Id == payload.SubjectId)
            .Select(x => new { x.Id, x.FarmId })
            .SingleOrDefaultAsync(cancellationToken);
        var stock = await (
            from lot in dbContext.VeterinaryMedicineStockLots
            join medicine in dbContext.VeterinaryMedicines on lot.MedicineId equals medicine.Id
            where lot.Id == payload.MedicineStockLotId
            select new
            {
                Lot = lot,
                medicine.FarmId,
                medicine.Name,
                medicine.Unit,
                medicine.DisplayUnit,
                medicine.MeatWithdrawalDays,
                medicine.MilkWithdrawalHours
            }).SingleOrDefaultAsync(cancellationToken);

        if (subject is null || stock is null || subject.FarmId != stock.FarmId)
        {
            throw new CommandRejectedException(
                "treatment_context_invalid",
                "The subject and medicine stock lot must belong to the same tenant and farm.");
        }
        if (!await scopeAccess.CanAccessFarmAsync(subject.FarmId, cancellationToken))
            throw new CommandRejectedException("treatment_context_invalid", "The livestock subject is unavailable in this organisation scope.");

        if (payload.Dose <= 0)
        {
            throw new CommandRejectedException("invalid_dose", "The treatment dose must be positive.");
        }

        DateOnly treatmentDate = DateOnly.FromDateTime(payload.AdministeredAt.UtcDateTime);
        if (stock.Lot.ExpiresOn is { } expiresOn && treatmentDate > expiresOn)
        {
            throw new CommandRejectedException("medicine_expired", "Expired medicine cannot be administered.");
        }

        Guid treatmentId = Guid.NewGuid();
        var treatment = new PlatformLivestockTreatment(
            treatmentId,
            payload.SubjectId,
            payload.MedicineStockLotId,
            payload.Dose,
            stock.DisplayUnit,
            payload.Route,
            payload.AdministeredAt,
            processedAt,
            stock.MeatWithdrawalDays,
            stock.MilkWithdrawalHours,
            command.CommandId,
            payload.Notes);
        if (treatment.Dose > stock.Lot.QuantityOnHand)
        {
            throw new CommandRejectedException("insufficient_medicine_stock", "The treatment dose exceeds medicine stock on hand.");
        }
        stock.Lot.Issue(treatment.Dose);
        dbContext.LivestockTreatments.Add(treatment);
        AddChangeAndOutbox(
            subject.FarmId,
            "LivestockTreatment",
            treatmentId,
            new
            {
                treatment.Id,
                treatment.SubjectId,
                treatment.MedicineStockLotId,
                Medicine = stock.Name,
                treatment.Dose,
                treatment.Unit,
                treatment.EnteredDose,
                treatment.EnteredUnit,
                treatment.DisplayUnit,
                treatment.Route,
                treatment.AdministeredAt,
                treatment.MeatHoldUntil,
                treatment.MilkHoldUntil,
                treatment.Notes,
                QuantityOnHand = stock.Lot.QuantityOnHand
            },
            processedAt);

        return treatmentId;
    }

    private async Task<Guid> RecordLivestockMovementAsync(
        SyncCommandEnvelope command, DateTimeOffset processedAt, CancellationToken cancellationToken)
    {
        RecordLivestockMovementPayload payload = command.Payload.Deserialize<RecordLivestockMovementPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A livestock movement payload is required.");
        Guid? farmId = await dbContext.LivestockSubjects.Where(x => x.Id == payload.SubjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            throw new CommandRejectedException("subject_not_found", "The livestock subject is unavailable in this tenant.");
        Guid[] holdingIds = new[] { payload.FromHoldingId, payload.ToHoldingId }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToArray();
        if (holdingIds.Length > 0 && await dbContext.LivestockHoldings.CountAsync(x => holdingIds.Contains(x.Id) && x.FarmId == farmId, cancellationToken) != holdingIds.Length)
            throw new CommandRejectedException("holding_not_found", "Movement holdings must belong to the subject's farm.");
        LivestockMovementRecord? latest = await dbContext.LivestockMovements.Where(x => x.SubjectId == payload.SubjectId)
            .OrderByDescending(x => x.OccurredAt).FirstOrDefaultAsync(cancellationToken);
        if (latest?.Type is LivestockMovementType.Sale or LivestockMovementType.Death)
            throw new CommandRejectedException("livestock_lifecycle_closed", "No movement can follow a sale or death record.");
        if (latest is not null && payload.OccurredAt < latest.OccurredAt)
            throw new CommandRejectedException("movement_out_of_order", "Use a correction workflow for historical movement changes.");
        if (payload.FromHoldingId is { } from && latest?.ToHoldingId != from)
            throw new CommandRejectedException("origin_mismatch", "Movement origin does not match the current holding.");
        Guid id = Guid.NewGuid();
        var movement = new LivestockMovementRecord(id, payload.SubjectId, payload.Type, payload.FromHoldingId,
            payload.ToHoldingId, payload.OccurredAt, payload.Reference, command.CommandId, payload.Notes);
        dbContext.LivestockMovements.Add(movement);
        AddChangeAndOutbox(farmId.Value, "LivestockMovement", id, new
        {
            movement.Id, movement.SubjectId, movement.Type, movement.FromHoldingId, movement.ToHoldingId,
            movement.OccurredAt, movement.Reference, movement.Notes
        }, processedAt);
        return id;
    }

    private async Task<Guid> RecordLivestockHealthEventAsync(
        SyncCommandEnvelope command, DateTimeOffset processedAt, CancellationToken cancellationToken)
    {
        RecordLivestockHealthEventPayload payload = command.Payload.Deserialize<RecordLivestockHealthEventPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A livestock health-event payload is required.");
        Guid? farmId = await dbContext.LivestockSubjects.Where(x => x.Id == payload.SubjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            throw new CommandRejectedException("subject_not_found", "The livestock subject is unavailable in this tenant.");
        Guid id = Guid.NewGuid();
        var healthEvent = new LivestockHealthEvent(id, payload.SubjectId, payload.Type, payload.Title,
            payload.OccurredAt, payload.Diagnosis, payload.Practitioner, command.CommandId, payload.Notes);
        dbContext.LivestockHealthEvents.Add(healthEvent);
        AddChangeAndOutbox(farmId.Value, "LivestockHealthEvent", id, new
        {
            healthEvent.Id, healthEvent.SubjectId, healthEvent.Type, healthEvent.Title, healthEvent.OccurredAt,
            healthEvent.Diagnosis, healthEvent.Practitioner, healthEvent.Notes
        }, processedAt);
        return id;
    }

    private async Task<Guid> AllocateLivestockFeedAsync(
        SyncCommandEnvelope command, DateTimeOffset processedAt, CancellationToken cancellationToken)
    {
        AllocateLivestockFeedPayload payload = command.Payload.Deserialize<AllocateLivestockFeedPayload>(JsonOptions)
            ?? throw new CommandRejectedException("invalid_payload", "A livestock feed-allocation payload is required.");
        Guid? farmId = await dbContext.LivestockSubjects.Where(x => x.Id == payload.SubjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            throw new CommandRejectedException("subject_not_found", "The livestock subject is unavailable in this tenant.");
        string lockKey = $"feed-batch:{payload.FeedBatchId:N}";
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
        LivestockFeedBatch? batch = await dbContext.LivestockFeedBatches
            .SingleOrDefaultAsync(x => x.Id == payload.FeedBatchId && x.FarmId == farmId, cancellationToken);
        if (batch is null) throw new CommandRejectedException("feed_batch_not_found", "The feed batch is unavailable for this subject.");
        if (batch.ExpiresOn is { } expiry && DateOnly.FromDateTime(payload.OccurredAt.UtcDateTime) > expiry)
            throw new CommandRejectedException("feed_batch_expired", $"Feed batch expired on {expiry:O}.");
        MeasuredQuantity measured = MeasurementUnits.Capture(payload.Quantity, payload.Unit, payload.Unit);
        if (!measured.CanonicalUnit.Equals(batch.Unit, StringComparison.OrdinalIgnoreCase))
            throw new CommandRejectedException("unit_mismatch", $"Feed batch is measured in {batch.Unit}.");
        if (measured.CanonicalValue > batch.QuantityOnHand)
            throw new CommandRejectedException("insufficient_feed_stock", $"Only {batch.QuantityOnHand} {batch.Unit} remains.");
        Guid id = Guid.NewGuid();
        var allocation = new LivestockFeedAllocation(id, payload.SubjectId, batch.Id, payload.Quantity,
            payload.Unit, payload.OccurredAt, command.CommandId, payload.Notes);
        batch.Issue(allocation.Quantity); dbContext.LivestockFeedAllocations.Add(allocation);
        AddChangeAndOutbox(farmId.Value, "LivestockFeedAllocation", id, new
        {
            allocation.Id, allocation.SubjectId, allocation.FeedBatchId, allocation.Quantity, allocation.Unit,
            allocation.EnteredQuantity, allocation.EnteredUnit, allocation.OccurredAt, allocation.Notes,
            batch.QuantityOnHand
        }, processedAt);
        return id;
    }

    private void AddChangeAndOutbox(Guid farmId, string entityType, Guid entityId, object payload,
        DateTimeOffset changedAt, string? eventType = null)
    {
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        dbContext.SyncChanges.Add(new SyncChange(
            tenantContext.TenantId,
            farmId,
            entityType,
            entityId,
            "Upsert",
            json,
            changedAt));
        dbContext.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(),
            tenantContext.TenantId,
            eventType ?? $"{entityType}.Recorded",
            json,
            changedAt));
    }

    private async Task<long> ExpectedSequenceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        long? last = await dbContext.SyncCommands
            .Where(x => x.DeviceId == deviceId)
            .Select(x => (long?)x.Sequence)
            .MaxAsync(cancellationToken);
        return (last ?? 0) + 1;
    }

    private async Task RollbackCommandAsync(
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction,
        string savepoint,
        CancellationToken cancellationToken)
    {
        await transaction.RollbackToSavepointAsync(savepoint, cancellationToken);
        dbContext.ChangeTracker.Clear();
    }

    private static SyncCommandResult ToDuplicateResult(SyncCommandLog command)
    {
        return new SyncCommandResult(
            command.Id,
            command.Sequence,
            command.Status == SyncCommandStatus.Applied ? "Duplicate" : "Rejected",
            command.ResultId,
            command.ErrorCode,
            command.ErrorMessage);
    }

    private sealed class CommandRejectedException(string code, string message) : Exception(message)
    {
        public string Code { get; } = code;
    }
}

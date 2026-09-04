using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Create;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderAssignment = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderAssignment;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;
using WorkResultType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResultType;

public sealed class CreateHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCommand message, CancellationToken cancellationToken) => CreateAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateAsync(Guid farmId, CreateWorkOrderRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken) || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        UseCaseResult? invalidTarget = await ValidateTargetAsync(farmId, request.FieldId, request.BoundaryVersionId,
            request.CropZoneId, request.LivestockSubjectId, request.CropPlanId, request.PlannedOperationId, db, cancellationToken);
        if (invalidTarget is not null) return invalidTarget;
        Guid id = NonEmptyOrNew(request.Id);
        string number = string.IsNullOrWhiteSpace(request.Number) ? MakeNumber(id, clock.GetUtcNow()) : request.Number;
        try
        {
            var order = new WorkOrder(id, farmId, number, request.Title, request.WorkType, request.Priority,
                request.ScheduledStart, request.ScheduledEnd, request.Instructions, actor.Subject, clock.GetUtcNow(),
                request.FieldId, request.BoundaryVersionId, request.CropZoneId, request.LivestockSubjectId,
                request.CropPlanId, request.PlannedOperationId);
            db.WorkOrders.Add(order);
            AddRecords(db, tenant, actor, order, "WorkOrderCreated", "WorkOrder.Created", clock.GetUtcNow(), context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/operations/work-orders/{id}", await BuildDetailAsync(order, db, cancellationToken));
        }
        catch (ArgumentException ex) { return Bad("work_order_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("work_order_conflict", DatabaseMessage(ex)); }
    }


    private static async Task<UseCaseResult?> ValidateTargetAsync(Guid farmId, Guid? fieldId, Guid? boundaryId,
        Guid? cropZoneId, Guid? livestockId, Guid? planId, Guid? operationId, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        if (fieldId is { } field && !await db.FarmFields.AnyAsync(x => x.Id == field && x.FarmId == farmId, cancellationToken)) return Bad("field_scope_invalid", "The field must belong to the work-order farm.");
        if (boundaryId is { } boundary && !await db.FieldBoundaryVersions.AnyAsync(x => x.Id == boundary && x.FieldId == fieldId, cancellationToken)) return Bad("boundary_scope_invalid", "The boundary version must belong to the selected field.");
        if (cropZoneId is { } zone && !await db.CropZones.AnyAsync(x => x.Id == zone && x.FieldId == fieldId, cancellationToken)) return Bad("crop_zone_scope_invalid", "The crop zone must belong to the selected field.");
        if (livestockId is { } livestock && !await db.LivestockSubjects.AnyAsync(x => x.Id == livestock && x.FarmId == farmId, cancellationToken)) return Bad("livestock_scope_invalid", "The livestock subject must belong to the work-order farm.");
        if (planId is { } plan && operationId is { } operation)
        {
            var linked = await (from p in db.CropPlans join o in db.PlannedCropOperations on p.Id equals o.PlanId
                join a in db.CropPlanAllocations on o.AllocationId equals a.Id
                where p.Id == plan && o.Id == operation
                select new { p.FarmId, p.Status, a.FieldId, a.BoundaryVersionId, a.CropZoneId }).SingleOrDefaultAsync(cancellationToken);
            if (linked is null || linked.FarmId != farmId || linked.Status != CropPlanStatus.Approved) return Bad("planned_operation_invalid", "The linked operation must belong to an approved plan for this farm.");
            if (linked.FieldId != fieldId || linked.BoundaryVersionId != boundaryId || linked.CropZoneId != cropZoneId) return Bad("planned_operation_target_mismatch", "The work-order field target must match its planned operation allocation.");
        }
        return null;
    }


    private static async Task<WorkOrderDetailResponse> BuildDetailAsync(WorkOrder order, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        WorkOrderAssignment[] assignments = await db.WorkOrderAssignments.AsNoTracking().Where(x => x.WorkOrderId == order.Id).OrderBy(x => x.AssignedAt).ToArrayAsync(cancellationToken);
        WorkOrderResource[] resources = await db.WorkOrderResources.AsNoTracking().Where(x => x.WorkOrderId == order.Id).OrderBy(x => x.CreatedAtUtc).ToArrayAsync(cancellationToken);
        WorkOrderStatusHistory[] history = await db.WorkOrderStatusHistory.AsNoTracking().Where(x => x.WorkOrderId == order.Id).OrderBy(x => x.OccurredAt).ToArrayAsync(cancellationToken);
        WorkOrderResult[] results = await db.WorkOrderResults.AsNoTracking().Where(x => x.WorkOrderId == order.Id).OrderBy(x => x.RecordedAt).ToArrayAsync(cancellationToken);
        return new WorkOrderDetailResponse(ToSummary(order, assignments.Count(x => x.UnassignedAt == null), resources.Where(x => x.Status != WorkResourceStatus.Released).Sum(x => x.PlannedCost)),
            order.Instructions, order.CreatedBy, order.DraftedAt, order.PlannedAt, order.AssignedAt, order.StartedAt,
            order.CompletedAt, order.VerifiedAt, order.VerifiedBy, order.RejectedAt, order.RejectedBy,
            order.RejectionReason, order.CancelledAt, order.CancelledBy, order.CancellationReason, order.CompletionNotes,
            assignments.Select(ToAssignment).ToArray(), resources.Select(ToResource).ToArray(),
            history.Select(x => new WorkOrderHistoryResponse(x.Id, x.FromStatus, x.ToStatus, x.OccurredAt, x.Actor, x.Reason)).ToArray(),
            results.Select(x => new WorkOrderResultResponse(x.Id, x.ResultType, x.ResultId, x.Notes, x.RecordedAt, x.RecordedBy)).ToArray());
    }

    private static WorkOrderSummaryResponse ToSummary(WorkOrder x, int assignees, decimal plannedCost) => new(x.Id, x.FarmId, x.Number,
        x.Title, x.WorkType, x.Priority, x.Status, x.ScheduledStart, x.ScheduledEnd, x.FieldId, x.BoundaryVersionId,
        x.CropZoneId, x.LivestockSubjectId, x.CropPlanId, x.PlannedOperationId, x.PreviousWorkOrderId,
        x.SupersededByWorkOrderId, assignees, decimal.Round(plannedCost, 2, MidpointRounding.AwayFromZero), x.Version);

    private static WorkOrderAssignmentResponse ToAssignment(WorkOrderAssignment x) => new(x.Id, x.MembershipId, x.AssignmentRole, x.IsPrimary, x.AssignedAt, x.AssignedBy, x.UnassignedAt, x.UnassignedBy);

    private static WorkOrderResourceResponse ToResource(WorkOrderResource x) => new(x.Id, x.ResourceType, x.ResourceName, x.ResourceReference,
        x.AgriculturalInputId, x.StockLotId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.UnitCost, x.Currency, x.PlannedCost, x.Status);

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        WorkOrder order, string action, string eventType, DateTimeOffset now, UseCaseContext context, object? additional = null)
    {
        object payload = new { order.Id, order.FarmId, order.Number, order.Status, order.ScheduledStart, order.ScheduledEnd,
            order.FieldId, order.CropZoneId, order.LivestockSubjectId, order.CropPlanId, order.PlannedOperationId, Additional = additional };
        string json = JsonSerializer.Serialize(payload);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, order.FarmId, nameof(WorkOrder), order.Id, action, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, nameof(WorkOrder), order.Id, order.FarmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static string MakeNumber(Guid id, DateTimeOffset now) => $"WO-{now:yyyyMMdd}-{id.ToString("N")[..8].ToUpperInvariant()}";

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DatabaseMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

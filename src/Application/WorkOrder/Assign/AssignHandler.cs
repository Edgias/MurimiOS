using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Assign;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using NotificationSeverity = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationSeverity;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
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

public sealed class AssignHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<AssignCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AssignCommand message, CancellationToken cancellationToken) => AssignAsync(message.workOrderId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> AssignAsync(Guid workOrderId, AssignWorkOrderRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        WorkOrder? order = await FindAccessibleAsync(workOrderId, db, scope, cancellationToken);
        if (order is null) return UseCaseResults.NotFound();
        if (order.Status is not (WorkOrderStatus.Planned or WorkOrderStatus.Assigned)) return Conflict("invalid_work_order_transition", "Only a planned or assigned work order can receive assignments.");
        TenantMembership? member = await db.TenantMemberships.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.MembershipId && x.Status == MembershipStatus.Active, cancellationToken);
        if (member is null || !await MembershipAllowsFarmAsync(member, order.FarmId, db, cancellationToken)) return Bad("assignee_scope_invalid", "The assignee must be active and allowed to access this farm.");
        bool duplicate = await db.WorkOrderAssignments.AnyAsync(x => x.WorkOrderId == order.Id && x.MembershipId == request.MembershipId && x.UnassignedAt == null, cancellationToken);
        if (duplicate) return Conflict("assignment_exists", "The membership already has an active assignment on this work order.");
        if (request.IsPrimary && await db.WorkOrderAssignments.AnyAsync(x => x.WorkOrderId == order.Id && x.IsPrimary && x.UnassignedAt == null, cancellationToken))
            return Conflict("primary_assignment_exists", "The work order already has a primary assignee.");
        if (await HasPersonConflictAsync(order, request.MembershipId, db, cancellationToken))
            return Conflict("assignee_schedule_conflict", "The assignee has another active work order during this schedule.");
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var assignment = new WorkOrderAssignment(NonEmptyOrNew(request.Id), order.Id, request.MembershipId,
                request.AssignmentRole, request.IsPrimary, now, actor.Subject);
            db.WorkOrderAssignments.Add(assignment);
            if (order.Status == WorkOrderStatus.Planned)
            {
                WorkOrderStatus previous = order.Status; order.MarkAssigned(now);
                AddHistory(db, order, previous, order.Status, actor.Subject, null, now);
            }
            db.Notifications.Add(new NotificationRecord(Guid.NewGuid(), request.MembershipId, "operations.assignment",
                $"Work order {order.Number} assigned", order.Title, NotificationSeverity.Information,
                nameof(WorkOrder), order.Id, $"/operations/work-orders/{order.Id}"));
            AddRecords(db, tenant, actor, order, "WorkOrderAssigned", "WorkOrder.Assigned", now, context,
                new { assignment.Id, assignment.MembershipId, assignment.AssignmentRole, assignment.IsPrimary });
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/operations/work-orders/{order.Id}/assignments/{assignment.Id}", await BuildDetailAsync(order, db, cancellationToken));
        }
        catch (ArgumentException ex) { return Bad("assignment_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("assignment_conflict", DatabaseMessage(ex)); }
    }


    private static async Task<WorkOrder?> FindAccessibleAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        WorkOrder? order = await db.WorkOrders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return order is not null && await scope.CanAccessFarmAsync(order.FarmId, cancellationToken) ? order : null;
    }

    private static async Task<bool> MembershipAllowsFarmAsync(TenantMembership member, Guid farmId, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        if (member.ScopeOrganisationNodeId is null) return true;
        var paths = await (from farm in db.Farms join node in db.OrganisationNodes on farm.OrganisationNodeId equals node.Id
            where farm.Id == farmId select new { FarmPath = node.Path }).SingleAsync(cancellationToken);
        string? scopePath = await db.OrganisationNodes.Where(x => x.Id == member.ScopeOrganisationNodeId).Select(x => x.Path).SingleOrDefaultAsync(cancellationToken);
        return scopePath is not null && (paths.FarmPath == scopePath || paths.FarmPath.StartsWith(scopePath + ".", StringComparison.Ordinal));
    }

    private static Task<bool> HasPersonConflictAsync(WorkOrder order, Guid membershipId, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.WorkOrders.AnyAsync(other => other.Id != order.Id && other.FarmId == order.FarmId
            && other.Status != WorkOrderStatus.Cancelled && other.Status != WorkOrderStatus.Superseded
            && other.ScheduledStart < order.ScheduledEnd && other.ScheduledEnd > order.ScheduledStart
            && db.WorkOrderAssignments.Any(a => a.WorkOrderId == other.Id && a.MembershipId == membershipId && a.UnassignedAt == null), cancellationToken);


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

    private static void AddHistory(IPlatformDbContext db, WorkOrder order, WorkOrderStatus from, WorkOrderStatus to, string actor, string? reason, DateTimeOffset now) =>
        db.WorkOrderStatusHistory.Add(new WorkOrderStatusHistory(Guid.NewGuid(), order.Id, from, to, now, actor, reason));

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

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DatabaseMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

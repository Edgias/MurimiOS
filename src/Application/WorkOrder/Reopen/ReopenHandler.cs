using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Reopen;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
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

public sealed class ReopenHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReopenCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReopenCommand message, CancellationToken cancellationToken) => ReopenAsync(message.workOrderId, db, scope, actor, tenant, clock, context, cancellationToken);

    private static Task<UseCaseResult> ReopenAsync(Guid workOrderId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken) =>        TransitionAsync(workOrderId, WorkOrderStatus.Assigned, "Reopened after rejection.", db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> TransitionAsync(Guid id, WorkOrderStatus target, string? reason,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        WorkOrder? order = await FindAccessibleAsync(id, db, scope, cancellationToken);
        if (order is null) return UseCaseResults.NotFound();
        if (target == WorkOrderStatus.InProgress && !actor.HasPermission(PlatformPermissions.OperationsAssign))
        {
            if (actor.MembershipId is not { } membershipId || !await db.WorkOrderAssignments.AnyAsync(x => x.WorkOrderId == id && x.MembershipId == membershipId && x.UnassignedAt == null, cancellationToken)) return UseCaseResults.Forbid();
        }
        DateTimeOffset now = clock.GetUtcNow(); WorkOrderStatus previous = order.Status;
        try
        {
            switch (target)
            {
                case WorkOrderStatus.Planned: order.Plan(now); break;
                case WorkOrderStatus.InProgress: order.Start(now); break;
                case WorkOrderStatus.Verified: order.Verify(now, actor.Subject); break;
                case WorkOrderStatus.Rejected: order.Reject(now, actor.Subject, reason!); break;
                case WorkOrderStatus.Assigned: order.Reopen(now); break;
                case WorkOrderStatus.Cancelled:
                    order.Cancel(now, actor.Subject, reason!);
                    foreach (WorkOrderResource resource in await db.WorkOrderResources.Where(x => x.WorkOrderId == id && x.Status == WorkResourceStatus.Reserved).ToArrayAsync(cancellationToken)) resource.Release();
                    break;
                default: throw new InvalidOperationException("Unsupported lifecycle transition.");
            }
            AddHistory(db, order, previous, order.Status, actor.Subject, reason, now);
            string action = $"WorkOrder{order.Status}"; AddRecords(db, tenant, actor, order, action, $"WorkOrder.{order.Status}", now, context, reason is null ? null : new { Reason = reason });
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Ok(await BuildDetailAsync(order, db, cancellationToken));
        }
        catch (ArgumentException ex) { return Bad("work_order_transition_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("invalid_work_order_transition", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("work_order_transition_conflict", DatabaseMessage(ex)); }
    }


    private static async Task<WorkOrder?> FindAccessibleAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        WorkOrder? order = await db.WorkOrders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return order is not null && await scope.CanAccessFarmAsync(order.FarmId, cancellationToken) ? order : null;
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

    private static string DatabaseMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

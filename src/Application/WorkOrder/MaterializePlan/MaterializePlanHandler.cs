using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.MaterializePlan;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;

public sealed class MaterializePlanHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<MaterializePlanCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(MaterializePlanCommand message, CancellationToken cancellationToken) => MaterializePlanAsync(message.planId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> MaterializePlanAsync(Guid planId, MaterializePlanRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        CropPlan? plan = await db.CropPlans.SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (plan.Status != CropPlanStatus.Approved) return Conflict("plan_not_approved", "Only an approved crop plan can be materialized as work orders.");
        var farm = await db.Farms.AsNoTracking().Where(x => x.Id == plan.FarmId).Select(x => new { x.TimeZone }).SingleAsync(cancellationToken);
        Guid[] selected = request.PlannedOperationIds?.Distinct().ToArray() ?? [];
        IQueryable<PlannedCropOperation> query = db.PlannedCropOperations.Where(x => x.PlanId == planId);
        if (selected.Length > 0) query = query.Where(x => selected.Contains(x.Id));
        PlannedCropOperation[] operations = await query.OrderBy(x => x.PlannedStart).ThenBy(x => x.Sequence).ToArrayAsync(cancellationToken);
        if (selected.Length > 0 && operations.Length != selected.Length) return Bad("planned_operation_invalid", "Every selected operation must belong to the plan.");
        Guid[] existing = await db.WorkOrders.Where(x => x.CropPlanId == planId && x.PlannedOperationId != null && x.Status != WorkOrderStatus.Superseded)
            .Select(x => x.PlannedOperationId!.Value).ToArrayAsync(cancellationToken);
        operations = operations.Where(x => !existing.Contains(x.Id)).ToArray();
        if (operations.Length == 0) return UseCaseResults.Ok(Array.Empty<WorkOrderSummaryResponse>());
        Guid[] allocationIds = operations.Select(x => x.AllocationId).Distinct().ToArray();
        CropPlanAllocation[] allocations = await db.CropPlanAllocations.Where(x => x.PlanId == planId && allocationIds.Contains(x.Id)).ToArrayAsync(cancellationToken);
        Dictionary<Guid, CropPlanAllocation> byAllocation = allocations.ToDictionary(x => x.Id);
        CropPlanResourceBudget[] budgets = await db.CropPlanResourceBudgets.Where(x => x.PlanId == planId && x.PlannedOperationId != null && operations.Select(o => o.Id).Contains(x.PlannedOperationId.Value)).ToArrayAsync(cancellationToken);
        DateTimeOffset now = clock.GetUtcNow(); var created = new List<WorkOrder>();
        foreach (PlannedCropOperation operation in operations)
        {
            CropPlanAllocation allocation = byAllocation[operation.AllocationId]; Guid id = Guid.NewGuid();
            var order = new WorkOrder(id, plan.FarmId, MakeNumber(id, now), $"{operation.OperationType} - {allocation.Crop}",
                operation.OperationType, request.Priority, LocalToUtc(operation.PlannedStart, farm.TimeZone),
                LocalToUtc(operation.PlannedEnd.AddDays(1), farm.TimeZone), operation.Notes ?? $"Generated from approved plan {plan.Name}.",
                actor.Subject, now, allocation.FieldId, allocation.BoundaryVersionId, allocation.CropZoneId,
                null, plan.Id, operation.Id);
            order.Plan(now); db.WorkOrders.Add(order); created.Add(order);
            AddHistory(db, order, WorkOrderStatus.Draft, WorkOrderStatus.Planned, actor.Subject, "Materialized from an approved crop plan.", now);
            foreach (CropPlanResourceBudget budget in budgets.Where(x => x.PlannedOperationId == operation.Id))
                db.WorkOrderResources.Add(new WorkOrderResource(Guid.NewGuid(), order.Id, (WorkResourceType)(int)budget.ResourceType,
                    budget.ResourceName, budget.EnteredQuantity, budget.EnteredUnit, budget.UnitCostPerCanonicalUnit,
                    budget.Currency, budget.AgriculturalInputId));
            AddRecords(db, tenant, actor, order, "PlanMaterialized", "WorkOrder.Materialized", now, context, new { plan.Id, PlannedOperationId = operation.Id });
        }
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/operations/work-orders?farmId={plan.FarmId}", created.Select(x => ToSummary(x, 0, budgets.Where(b => b.PlannedOperationId == x.PlannedOperationId).Sum(b => b.PlannedCost))).ToArray());
        }
        catch (DbUpdateException ex) { return Conflict("plan_materialization_conflict", DatabaseMessage(ex)); }
    }

    private static WorkOrderSummaryResponse ToSummary(WorkOrder x, int assignees, decimal plannedCost) => new(x.Id, x.FarmId, x.Number,
        x.Title, x.WorkType, x.Priority, x.Status, x.ScheduledStart, x.ScheduledEnd, x.FieldId, x.BoundaryVersionId,
        x.CropZoneId, x.LivestockSubjectId, x.CropPlanId, x.PlannedOperationId, x.PreviousWorkOrderId,
        x.SupersededByWorkOrderId, assignees, decimal.Round(plannedCost, 2, MidpointRounding.AwayFromZero), x.Version);

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

    private static DateTimeOffset LocalToUtc(DateOnly date, string timeZone)
    {
        DateTime local = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, ResolveTimeZone(timeZone)), TimeSpan.Zero);
    }

    private static TimeZoneInfo ResolveTimeZone(string id) { try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; } catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; } }

    private static string MakeNumber(Guid id, DateTimeOffset now) => $"WO-{now:yyyyMMdd}-{id.ToString("N")[..8].ToUpperInvariant()}";

    private static string DatabaseMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

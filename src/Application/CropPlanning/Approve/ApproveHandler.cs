using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.Approve;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ApproveHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ApproveCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ApproveCommand message, CancellationToken cancellationToken) => ApproveAsync(message.planId, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ApproveAsync(Guid planId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        CropPlan? plan = await db.CropPlans.SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (plan.Status != CropPlanStatus.Submitted) return Conflict("invalid_plan_transition", "Only a submitted plan can be approved.");
        DateTimeOffset now = clock.GetUtcNow();
        CropPlan[] existing = await db.CropPlans.Where(x => x.Id != plan.Id && x.FarmId == plan.FarmId
            && x.SeasonId == plan.SeasonId && x.Status == CropPlanStatus.Approved).ToArrayAsync(cancellationToken);
        foreach (CropPlan approved in existing) approved.Supersede(plan.Id, now);
        if (existing.Length > 0) await db.SaveChangesAsync(cancellationToken);
        plan.Approve(now, actor.Subject);
        AddLifecycleRecords(db, tenant, actor, plan, "PlanApproved", "CropPlan.Approved", now, context);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return UseCaseResults.Ok(ToSummary(plan, await db.CropPlanAllocations.Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken),
            await db.CropPlanResourceBudgets.Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken)));
    }


    private static CropPlanSummaryResponse ToSummary(CropPlan plan, IEnumerable<CropPlanAllocation> allocations,
        IEnumerable<CropPlanResourceBudget> resources)
    {
        CropPlanAllocation[] allocationRows = allocations.ToArray(); CropPlanResourceBudget[] resourceRows = resources.ToArray();
        decimal cost = resourceRows.Sum(x => x.PlannedCost); decimal revenue = allocationRows.Sum(x => x.PlannedRevenue);
        return new CropPlanSummaryResponse(plan.Id, plan.PlanningGroupId, plan.ScenarioId, plan.FarmId, plan.SeasonId,
            plan.Name, plan.ScenarioName, plan.Currency, plan.Revision, plan.PreviousPlanId, plan.Status,
            allocationRows.Sum(x => x.PlannedAreaHa), cost, revenue, revenue - cost, plan.DraftedAt,
            plan.SubmittedAt, plan.ApprovedAt, plan.RejectedAt, plan.RejectionReason, plan.SupersededAt, plan.SupersededByPlanId);
    }


    private static void AddLifecycleRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        CropPlan plan, string action, string eventType, DateTimeOffset now, UseCaseContext context, object? additional = null)
    {
        object payload = new { plan.Id, plan.PlanningGroupId, plan.ScenarioId, plan.FarmId, plan.SeasonId,
            plan.Revision, plan.Status, Additional = additional };
        AddChange(db, tenant.TenantId, plan.FarmId, "CropPlan", plan.Id, action, payload, now);
        AddAudit(db, tenant, actor, action, plan, plan.FarmId, payload, now, context);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, JsonSerializer.Serialize(payload), now));
    }

    private static void AddChange(IPlatformDbContext db, Guid tenantId, Guid farmId, string entityType,
        Guid entityId, string operation, object payload, DateTimeOffset now) =>
        db.SyncChanges.Add(new SyncChange(tenantId, farmId, entityType, entityId, operation, JsonSerializer.Serialize(payload), now));

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, CropPlan? plan, Guid farmId, object changes, DateTimeOffset now, UseCaseContext context, Guid? entityId = null) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, plan is null ? nameof(CropProductApplicationRule) : nameof(CropPlan), entityId ?? plan!.Id,
            farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

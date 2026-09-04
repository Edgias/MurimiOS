using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.CreatePlan;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreatePlanHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreatePlanCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreatePlanCommand message, CancellationToken cancellationToken) => CreatePlanAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreatePlanAsync(Guid farmId, CreateCropPlanRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        var seasonContext = await (from season in db.GrowingSeasons
            join farm in db.Farms on season.FarmId equals farm.Id
            where season.Id == request.SeasonId && season.FarmId == farmId
            select new { Season = season, Farm = farm }).SingleOrDefaultAsync(cancellationToken);
        if (seasonContext is null) return UseCaseResults.NotFound();

        Guid planningGroupId = request.PlanningGroupId is { } groupId && groupId != Guid.Empty ? groupId : Guid.NewGuid();
        if (request.PlanningGroupId is not null && !await db.CropPlans.AnyAsync(x => x.PlanningGroupId == planningGroupId
            && x.FarmId == farmId && x.SeasonId == request.SeasonId, cancellationToken))
            return Bad("planning_group_invalid", "The planning group does not belong to this farm and season.");
        Guid id = NonEmptyOrNew(request.Id);
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var plan = new CropPlan(id, planningGroupId, id, farmId, request.SeasonId,
                request.Name, request.ScenarioName, request.Currency, 1, null, now, actor.Subject);
            db.CropPlans.Add(plan);
            AddChange(db, tenant.TenantId, farmId, "CropPlan", id, "Created", new { plan.Id, plan.Name, plan.ScenarioName, plan.Revision, plan.Status }, now);
            AddAudit(db, tenant, actor, "PlanCreated", plan, farmId, new { plan.Name, plan.ScenarioName, plan.Currency }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/crop-planning/plans/{id}", ToSummary(plan, [], []));
        }
        catch (ArgumentException ex) { return Bad("crop_plan_invalid", ex.Message); }
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

    private static void AddChange(IPlatformDbContext db, Guid tenantId, Guid farmId, string entityType,
        Guid entityId, string operation, object payload, DateTimeOffset now) =>
        db.SyncChanges.Add(new SyncChange(tenantId, farmId, entityType, entityId, operation, JsonSerializer.Serialize(payload), now));

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, CropPlan? plan, Guid farmId, object changes, DateTimeOffset now, UseCaseContext context, Guid? entityId = null) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, plan is null ? nameof(CropProductApplicationRule) : nameof(CropPlan), entityId ?? plan!.Id,
            farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

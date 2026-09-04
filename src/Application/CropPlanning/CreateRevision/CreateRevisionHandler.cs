using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.CreateRevision;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateRevisionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateRevisionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRevisionCommand message, CancellationToken cancellationToken) => CreateRevisionAsync(message.planId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateRevisionAsync(Guid planId, CreateCropPlanRevisionRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        CropPlan? source = await db.CropPlans.AsNoTracking().SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (source is null || !await scope.CanAccessFarmAsync(source.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (source.Status is CropPlanStatus.Draft or CropPlanStatus.Submitted)
            return Conflict("revision_not_allowed", "Submit, reject, or approve the current plan before creating a revision.");
        int revision = await db.CropPlans.Where(x => x.ScenarioId == source.ScenarioId).MaxAsync(x => x.Revision, cancellationToken) + 1;
        Guid newId = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var replacement = new CropPlan(newId, source.PlanningGroupId, source.ScenarioId, source.FarmId,
                source.SeasonId, request.Name ?? source.Name, source.ScenarioName, source.Currency, revision,
                source.Id, now, actor.Subject);
            CropPlanAllocation[] oldAllocations = await db.CropPlanAllocations.AsNoTracking().Where(x => x.PlanId == source.Id).ToArrayAsync(cancellationToken);
            PlannedCropOperation[] oldOperations = await db.PlannedCropOperations.AsNoTracking().Where(x => x.PlanId == source.Id).ToArrayAsync(cancellationToken);
            CropPlanResourceBudget[] oldResources = await db.CropPlanResourceBudgets.AsNoTracking().Where(x => x.PlanId == source.Id).ToArrayAsync(cancellationToken);
            Dictionary<Guid, Guid> allocationIds = oldAllocations.ToDictionary(x => x.Id, _ => Guid.NewGuid());
            Dictionary<Guid, Guid> operationIds = oldOperations.ToDictionary(x => x.Id, _ => Guid.NewGuid());
            CropPlanAllocation[] allocations = oldAllocations.Select(x => new CropPlanAllocation(allocationIds[x.Id], newId,
                x.FieldId, x.BoundaryVersionId, x.CropZoneId, x.Crop, x.Variety, x.EnteredArea, x.EnteredAreaUnit,
                x.EnteredTargetYield, x.EnteredTargetYieldUnit, x.TargetPricePerCanonicalUnit, x.Currency,
                x.PlannedSowingDate, x.PlannedHarvestDate)).ToArray();
            PlannedCropOperation[] operations = oldOperations.Select(x => new PlannedCropOperation(operationIds[x.Id], newId,
                allocationIds[x.AllocationId], x.OperationType, x.PlannedStart, x.PlannedEnd, x.Sequence, x.Notes)).ToArray();
            CropPlanResourceBudget[] resources = oldResources.Select(x => new CropPlanResourceBudget(Guid.NewGuid(), newId,
                allocationIds[x.AllocationId], x.PlannedOperationId is { } operationId ? operationIds[operationId] : null,
                x.ResourceType, x.AgriculturalInputId, x.ResourceName, x.EnteredQuantity, x.EnteredUnit,
                x.UnitCostPerCanonicalUnit, x.Currency)).ToArray();
            db.CropPlans.Add(replacement); db.CropPlanAllocations.AddRange(allocations);
            db.PlannedCropOperations.AddRange(operations); db.CropPlanResourceBudgets.AddRange(resources);
            AddChange(db, tenant.TenantId, source.FarmId, "CropPlan", newId, "RevisionCreated",
                new { replacement.Id, replacement.PreviousPlanId, replacement.Revision }, now);
            AddAudit(db, tenant, actor, "RevisionCreated", replacement, source.FarmId,
                new { replacement.PreviousPlanId, replacement.Revision }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/crop-planning/plans/{newId}", await BuildDetailAsync(replacement, db, cancellationToken));
        }
        catch (ArgumentException ex) { return Bad("revision_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("revision_conflict", "A concurrent revision was created; reload and retry."); }
    }


    private static async Task<CropPlanDetailResponse> BuildDetailAsync(CropPlan plan, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        CropPlanAllocation[] allocations = await db.CropPlanAllocations.AsNoTracking().Where(x => x.PlanId == plan.Id)
            .OrderBy(x => x.FieldId).ThenBy(x => x.Crop).ToArrayAsync(cancellationToken);
        PlannedCropOperation[] operations = await db.PlannedCropOperations.AsNoTracking().Where(x => x.PlanId == plan.Id)
            .OrderBy(x => x.AllocationId).ThenBy(x => x.Sequence).ToArrayAsync(cancellationToken);
        CropPlanResourceBudget[] resources = await db.CropPlanResourceBudgets.AsNoTracking().Where(x => x.PlanId == plan.Id)
            .OrderBy(x => x.AllocationId).ThenBy(x => x.ResourceType).ToArrayAsync(cancellationToken);
        return new CropPlanDetailResponse(ToSummary(plan, allocations, resources),
            allocations.Select(ToAllocation).ToArray(), operations.Select(ToOperation).ToArray(), resources.Select(ToResource).ToArray());
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

    private static CropPlanAllocationResponse ToAllocation(CropPlanAllocation x) => new(x.Id, x.PlanId, x.FieldId,
        x.BoundaryVersionId, x.CropZoneId, x.Crop, x.Variety, x.PlannedAreaHa, x.EnteredArea, x.EnteredAreaUnit,
        x.TargetYield, x.TargetYieldUnit, x.EnteredTargetYield, x.EnteredTargetYieldUnit,
        x.TargetPricePerCanonicalUnit, x.Currency, x.PlannedRevenue, x.PlannedSowingDate, x.PlannedHarvestDate);

    private static PlannedCropOperationResponse ToOperation(PlannedCropOperation x) => new(x.Id, x.PlanId,
        x.AllocationId, x.OperationType, x.PlannedStart, x.PlannedEnd, x.Sequence, x.Notes);

    private static CropPlanResourceBudgetResponse ToResource(CropPlanResourceBudget x) => new(x.Id, x.PlanId,
        x.AllocationId, x.PlannedOperationId, x.ResourceType, x.AgriculturalInputId, x.ResourceName,
        x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.UnitCostPerCanonicalUnit, x.Currency, x.PlannedCost);

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

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

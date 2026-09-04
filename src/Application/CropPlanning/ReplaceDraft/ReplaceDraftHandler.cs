using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ReplaceDraftHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReplaceDraftCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReplaceDraftCommand message, CancellationToken cancellationToken) => ReplaceDraftAsync(message.planId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReplaceDraftAsync(Guid planId, ReplaceCropPlanDraftRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        CropPlan? plan = await db.CropPlans.SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        try { plan.EnsureDraft(); }
        catch (InvalidOperationException ex) { return Conflict("plan_not_editable", ex.Message); }
        if (request.Allocations is null || request.Operations is null || request.Resources is null)
            return Bad("plan_lines_required", "Allocations, operations, and resources must be supplied as arrays.");
        if (HasDuplicateIds(request.Allocations.Select(x => x.Id)) || HasDuplicateIds(request.Operations.Select(x => x.Id))
            || HasDuplicateIds(request.Resources.Select(x => x.Id)))
            return Bad("duplicate_plan_line", "Plan line identifiers must be unique and non-empty.");

        var season = await db.GrowingSeasons.AsNoTracking().SingleAsync(x => x.Id == plan.SeasonId, cancellationToken);
        string country = await db.Farms.Where(x => x.Id == plan.FarmId).Select(x => x.CountryCode).SingleAsync(cancellationToken);
        Guid[] fieldIds = request.Allocations.Select(x => x.FieldId).Distinct().ToArray();
        var fieldRows = await db.FarmFields.AsNoTracking().Where(x => fieldIds.Contains(x.Id) && x.FarmId == plan.FarmId)
            .Select(x => new { x.Id, x.FarmId }).ToArrayAsync(cancellationToken);
        if (fieldRows.Length != fieldIds.Length) return Bad("field_scope_invalid", "Every allocation field must belong to the plan farm.");
        Guid[] boundaryIds = request.Allocations.Select(x => x.BoundaryVersionId).Distinct().ToArray();
        var boundaries = await db.FieldBoundaryVersions.AsNoTracking().Where(x => boundaryIds.Contains(x.Id))
            .Select(x => new { x.Id, x.FieldId, x.AreaHa }).ToDictionaryAsync(x => x.Id, cancellationToken);
        Guid[] zoneIds = request.Allocations.Where(x => x.CropZoneId is not null).Select(x => x.CropZoneId!.Value).Distinct().ToArray();
        var zones = await db.CropZones.AsNoTracking().Where(x => zoneIds.Contains(x.Id) && x.SeasonId == plan.SeasonId)
            .Select(x => new { x.Id, x.FieldId, x.Crop, x.Variety }).ToDictionaryAsync(x => x.Id, cancellationToken);
        if (zones.Count != zoneIds.Length) return Bad("crop_zone_scope_invalid", "Every crop zone must belong to the plan season.");

        CropPlanAllocation[] allocations;
        PlannedCropOperation[] operations;
        CropPlanResourceBudget[] resources;
        try
        {
            allocations = request.Allocations.Select(line => new CropPlanAllocation(line.Id, plan.Id, line.FieldId,
                line.BoundaryVersionId, line.CropZoneId, line.Crop, line.Variety, line.PlannedArea, line.AreaUnit,
                line.TargetYield, line.TargetYieldUnit, line.TargetPricePerCanonicalUnit, plan.Currency,
                line.PlannedSowingDate, line.PlannedHarvestDate)).ToArray();
            foreach (CropPlanAllocation allocation in allocations)
            {
                if (!boundaries.TryGetValue(allocation.BoundaryVersionId, out var boundary) || boundary.FieldId != allocation.FieldId)
                    return Bad("boundary_scope_invalid", "Every allocation boundary must belong to its field.");
                if (allocation.PlannedAreaHa > boundary.AreaHa)
                    return Bad("planned_area_exceeds_field", "Planned area cannot exceed the selected boundary area.");
                if (allocation.PlannedSowingDate < season.StartsOn || allocation.PlannedHarvestDate > season.EndsOn)
                    return Bad("allocation_outside_season", "Sowing and harvest dates must fall within the growing season.");
                if (allocation.CropZoneId is { } zoneId)
                {
                    var zone = zones[zoneId];
                    if (zone.FieldId != allocation.FieldId || !zone.Crop.Equals(allocation.Crop, StringComparison.OrdinalIgnoreCase)
                        || !string.Equals(zone.Variety, allocation.Variety, StringComparison.OrdinalIgnoreCase))
                        return Bad("crop_zone_mismatch", "A linked crop zone must match the allocation field, crop, and variety.");
                }
            }
            foreach (IGrouping<Guid, CropPlanAllocation> group in allocations.GroupBy(x => x.FieldId))
            {
                if (group.Any(x => x.CropZoneId is null) && group.Count() > 1)
                    return Bad("ambiguous_field_allocation", "A field may have multiple allocations only when every allocation is linked to a crop zone.");
                decimal boundaryArea = boundaries[group.First().BoundaryVersionId].AreaHa;
                if (group.Sum(x => x.PlannedAreaHa) > boundaryArea)
                    return Bad("planned_area_exceeds_field", "Combined allocations cannot exceed the field boundary area.");
            }
            Dictionary<Guid, CropPlanAllocation> allocationById = allocations.ToDictionary(x => x.Id);
            operations = request.Operations.Select(line => new PlannedCropOperation(line.Id, plan.Id, line.AllocationId,
                line.OperationType, line.PlannedStart, line.PlannedEnd, line.Sequence, line.Notes)).ToArray();
            foreach (PlannedCropOperation operation in operations)
            {
                if (!allocationById.TryGetValue(operation.AllocationId, out CropPlanAllocation? allocation))
                    return Bad("operation_allocation_invalid", "Every planned operation must reference an allocation in this draft.");
                if (operation.PlannedStart < season.StartsOn || operation.PlannedEnd > season.EndsOn)
                    return Bad("operation_outside_season", "Planned operation dates must fall within the growing season.");
            }
            Dictionary<Guid, PlannedCropOperation> operationById = operations.ToDictionary(x => x.Id);
            resources = request.Resources.Select(line => new CropPlanResourceBudget(line.Id, plan.Id, line.AllocationId,
                line.PlannedOperationId, line.ResourceType, line.AgriculturalInputId, line.ResourceName,
                line.Quantity, line.Unit, line.UnitCostPerCanonicalUnit, plan.Currency)).ToArray();
            Guid[] inputIds = resources.Where(x => x.AgriculturalInputId is not null).Select(x => x.AgriculturalInputId!.Value).Distinct().ToArray();
            AgriculturalInput[] inputs = await db.AgriculturalInputs.AsNoTracking()
                .Where(x => inputIds.Contains(x.Id) && x.FarmId == plan.FarmId).ToArrayAsync(cancellationToken);
            if (inputs.Length != inputIds.Length) return Bad("input_scope_invalid", "Every planned agricultural input must belong to the plan farm.");
            CropProductApplicationRule[] rules = await db.CropProductApplicationRules.AsNoTracking()
                .Where(x => inputIds.Contains(x.InputId) && x.Jurisdiction == country.ToUpper()).ToArrayAsync(cancellationToken);
            foreach (CropPlanResourceBudget resource in resources)
            {
                if (!allocationById.TryGetValue(resource.AllocationId, out CropPlanAllocation? allocation))
                    return Bad("resource_allocation_invalid", "Every resource must reference an allocation in this draft.");
                PlannedCropOperation? operation = resource.PlannedOperationId is { } operationId && operationById.TryGetValue(operationId, out var found) ? found : null;
                if (resource.PlannedOperationId is not null && (operation is null || operation.AllocationId != resource.AllocationId))
                    return Bad("resource_operation_invalid", "A linked operation must belong to the same allocation.");
                if (resource.ResourceType == PlannedResourceType.Input)
                {
                    if (operation is null) return Bad("input_operation_required", "An input budget must reference its planned application operation.");
                    CropProductApplicationRule? rule = rules.Where(x => x.InputId == resource.AgriculturalInputId
                        && x.Crop.Equals(allocation.Crop, StringComparison.OrdinalIgnoreCase)
                        && x.EffectiveFrom <= operation.PlannedStart
                        && (x.EffectiveTo == null || x.EffectiveTo >= operation.PlannedStart))
                        .OrderByDescending(x => x.EffectiveFrom).FirstOrDefault();
                    if (rule is null) return Bad("product_not_authorized", "No effective product authorization covers the crop, jurisdiction, and application date.");
                    rule.EnsurePermitted(allocation, resource, operation.PlannedStart);
                    rule.EnsureReEntryInterval(operation, operations.Where(x => x.AllocationId == allocation.Id));
                }
            }
        }
        catch (ArgumentException ex) { return Bad("crop_plan_draft_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Bad("product_constraint_failed", ex.Message); }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            CropPlanResourceBudget[] oldResources = await db.CropPlanResourceBudgets.Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
            PlannedCropOperation[] oldOperations = await db.PlannedCropOperations.Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
            CropPlanAllocation[] oldAllocations = await db.CropPlanAllocations.Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
            db.CropPlanResourceBudgets.RemoveRange(oldResources);
            db.PlannedCropOperations.RemoveRange(oldOperations);
            db.CropPlanAllocations.RemoveRange(oldAllocations);
            await db.SaveChangesAsync(cancellationToken);
            db.CropPlanAllocations.AddRange(allocations);
            db.PlannedCropOperations.AddRange(operations);
            db.CropPlanResourceBudgets.AddRange(resources);
            DateTimeOffset now = clock.GetUtcNow();
            AddChange(db, tenant.TenantId, plan.FarmId, "CropPlan", plan.Id, "DraftReplaced",
                new { plan.Id, Allocations = allocations.Length, Operations = operations.Length, Resources = resources.Length }, now);
            AddAudit(db, tenant, actor, "DraftReplaced", plan, plan.FarmId,
                new { Allocations = allocations.Length, Operations = operations.Length, Resources = resources.Length }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return UseCaseResults.Ok(await BuildDetailAsync(plan, db, cancellationToken));
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Conflict("crop_plan_conflict", ex.InnerException?.Message ?? ex.Message);
        }
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

    private static bool HasDuplicateIds(IEnumerable<Guid> ids) { Guid[] values = ids.ToArray(); return values.Any(x => x == Guid.Empty) || values.Distinct().Count() != values.Length; }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

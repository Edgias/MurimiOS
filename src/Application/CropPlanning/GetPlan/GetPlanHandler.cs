using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.GetPlan;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;

public sealed class GetPlanHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetPlanQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetPlanQuery message, CancellationToken cancellationToken) => GetPlanAsync(message.planId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetPlanAsync(Guid planId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        CropPlan? plan = await db.CropPlans.AsNoTracking().SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(await BuildDetailAsync(plan, db, cancellationToken));
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

}

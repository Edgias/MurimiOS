using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ListPlans;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;

public sealed class ListPlansHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListPlansQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListPlansQuery message, CancellationToken cancellationToken) => ListPlansAsync(message.farmId, message.seasonId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListPlansAsync(Guid farmId, Guid? seasonId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        IQueryable<CropPlan> query = db.CropPlans.AsNoTracking().Where(x => x.FarmId == farmId);
        if (seasonId is { } id) query = query.Where(x => x.SeasonId == id);
        CropPlan[] plans = await query.OrderByDescending(x => x.DraftedAt).ToArrayAsync(cancellationToken);
        Guid[] planIds = plans.Select(x => x.Id).ToArray();
        CropPlanAllocation[] allocations = await db.CropPlanAllocations.AsNoTracking().Where(x => planIds.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        CropPlanResourceBudget[] budgets = await db.CropPlanResourceBudgets.AsNoTracking().Where(x => planIds.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(plans.Select(plan => ToSummary(plan,
            allocations.Where(x => x.PlanId == plan.Id), budgets.Where(x => x.PlanId == plan.Id))).ToArray());
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

}

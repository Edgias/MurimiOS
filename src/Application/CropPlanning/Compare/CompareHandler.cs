using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.Compare;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;


using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;

public sealed class CompareHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CompareCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CompareCommand message, CancellationToken cancellationToken) => CompareAsync(message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CompareAsync(CompareCropPlansRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid[] ids = request.PlanIds?.Where(x => x != Guid.Empty).Distinct().ToArray() ?? [];
        if (ids.Length is < 2 or > 6) return Bad("comparison_size_invalid", "Compare between two and six plan scenarios.");
        CropPlan[] plans = await db.CropPlans.AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);
        if (plans.Length != ids.Length || plans.Select(x => (x.FarmId, x.SeasonId, x.PlanningGroupId)).Distinct().Count() != 1)
            return Bad("comparison_scope_invalid", "Plans must belong to the same planning group, farm, and season.");
        if (!await scope.CanAccessFarmAsync(plans[0].FarmId, cancellationToken)) return UseCaseResults.NotFound();
        var results = new List<CropPlanVarianceResponse>();
        foreach (CropPlan plan in plans.OrderBy(x => Array.IndexOf(ids, x.Id)))
            results.Add(await BuildVarianceAsync(plan, db, cancellationToken));
        return UseCaseResults.Ok(new CropPlanComparisonResponse(plans[0].PlanningGroupId, plans[0].FarmId, plans[0].SeasonId, results));
    }


    private static async Task<CropPlanVarianceResponse> BuildVarianceAsync(CropPlan plan, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        CropPlanAllocation[] allocations = await db.CropPlanAllocations.AsNoTracking().Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
        PlannedCropOperation[] plannedOperations = await db.PlannedCropOperations.AsNoTracking().Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
        CropPlanResourceBudget[] budgets = await db.CropPlanResourceBudgets.AsNoTracking().Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
        Guid[] lineagePlanIds = await db.CropPlans.AsNoTracking()
            .Where(x => x.ScenarioId == plan.ScenarioId && x.Revision <= plan.Revision)
            .Select(x => x.Id).ToArrayAsync(cancellationToken);
        CropPlanAllocation[] lineageAllocations = await db.CropPlanAllocations.AsNoTracking()
            .Where(x => lineagePlanIds.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        CropPlanFinancialEntry[] financials = await db.CropPlanFinancialEntries.AsNoTracking()
            .Where(x => lineagePlanIds.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        var season = await db.GrowingSeasons.AsNoTracking().SingleAsync(x => x.Id == plan.SeasonId, cancellationToken);
        DateTimeOffset rangeStart = new(season.StartsOn.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        DateTimeOffset rangeEnd = new(season.EndsOn.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        CostCentre[] financeCentres = await db.CostCentres.AsNoTracking().Where(x => x.FarmId == plan.FarmId
            && x.Currency == plan.Currency && (x.Type == CostCentreType.Field || x.Type == CostCentreType.CropZone))
            .ToArrayAsync(cancellationToken);
        FinanceTransaction[] financeTransactions = await db.FinanceTransactions.AsNoTracking()
            .Where(x => x.FarmId == plan.FarmId && x.Currency == plan.Currency
                && x.OccurredAt >= rangeStart && x.OccurredAt < rangeEnd).ToArrayAsync(cancellationToken);
        Guid[] financeTransactionIds = financeTransactions.Select(x => x.Id).ToArray();
        CostAllocation[] financeAllocations = await db.CostAllocations.AsNoTracking()
            .Where(x => financeTransactionIds.Contains(x.TransactionId)).ToArrayAsync(cancellationToken);
        var financeTransactionById = financeTransactions.ToDictionary(x => x.Id);
        Guid[] planningFinanceTransactionIds = financeTransactions.Where(x => x.SourceType == FinanceSourceType.Planning)
            .Select(x => x.Id).ToArray();
        HashSet<Guid> planningLedgerTransactionIds = financeTransactions
            .Where(x => x.SourceType == FinanceSourceType.Planning
                || x.ReversalOfId is { } reversalOfId && planningFinanceTransactionIds.Contains(reversalOfId))
            .Select(x => x.Id).ToHashSet();
        HashSet<Guid> linkedPlanningEntryIds = financeTransactions.Where(x => x.SourceType == FinanceSourceType.Planning
                && x.SourceId is not null).Select(x => x.SourceId!.Value).ToHashSet();
        Guid[] fieldIds = allocations.Select(x => x.FieldId).Distinct().ToArray();
        Guid[] zoneIds = allocations.Where(x => x.CropZoneId is not null).Select(x => x.CropZoneId!.Value).Distinct().ToArray();
        var actualOperations = await db.FieldOperations.AsNoTracking().Where(x => x.SupersededById == null
            && x.OccurredAt >= rangeStart && x.OccurredAt < rangeEnd && fieldIds.Contains(x.FieldId))
            .Select(x => new { x.Id, x.FieldId, x.CropZoneId, x.OperationType }).ToArrayAsync(cancellationToken);
        var actualInputs = await (from movement in db.InventoryMovements.AsNoTracking()
            join operation in db.FieldOperations.AsNoTracking() on movement.OperationId equals (Guid?)operation.Id
            join lot in db.InputStockLots.AsNoTracking() on movement.StockLotId equals lot.Id
            join input in db.AgriculturalInputs.AsNoTracking() on lot.InputId equals input.Id
            where movement.Type == InventoryMovementType.Issue && movement.OccurredAt >= rangeStart && movement.OccurredAt < rangeEnd
                && operation.SupersededById == null && fieldIds.Contains(operation.FieldId)
            select new { input.Id, operation.FieldId, operation.CropZoneId, movement.Quantity, movement.Unit }).ToArrayAsync(cancellationToken);
        var harvests = await db.HarvestLots.AsNoTracking().Where(x => zoneIds.Contains(x.CropZoneId)
            && x.HarvestedAt >= rangeStart && x.HarvestedAt < rangeEnd)
            .Select(x => new { x.CropZoneId, x.Quantity, x.Unit }).ToArrayAsync(cancellationToken);

        var allocationRows = new List<CropPlanAllocationVarianceResponse>();
        foreach (CropPlanAllocation allocation in allocations)
        {
            PlannedCropOperation[] allocationPlans = plannedOperations.Where(x => x.AllocationId == allocation.Id).ToArray();
            var allocationActualOps = actualOperations.Where(x => x.FieldId == allocation.FieldId
                && (allocation.CropZoneId is null ? x.CropZoneId is null : x.CropZoneId == allocation.CropZoneId)).ToArray();
            CropPlanResourceBudget[] allocationBudgets = budgets.Where(x => x.AllocationId == allocation.Id).ToArray();
            var resourceRows = allocationBudgets.Select(resource =>
            {
                var matching = actualInputs.Where(x => resource.AgriculturalInputId == x.Id && x.FieldId == allocation.FieldId
                    && (allocation.CropZoneId is null ? x.CropZoneId is null : x.CropZoneId == allocation.CropZoneId)
                    && x.Unit == resource.Unit).ToArray();
                decimal? actual = resource.ResourceType == PlannedResourceType.Input ? matching.Sum(x => x.Quantity) : null;
                return new CropPlanResourceVarianceResponse(resource.Id, resource.ResourceType, resource.ResourceName,
                    resource.AgriculturalInputId, resource.Quantity, actual, actual is null ? null : actual.Value - resource.Quantity,
                    resource.Unit, resource.PlannedCost, resource.Currency);
            }).ToArray();
            Guid[] lineageAllocationIds = lineageAllocations.Where(x => x.FieldId == allocation.FieldId
                && x.CropZoneId == allocation.CropZoneId
                && x.Crop.Equals(allocation.Crop, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Variety, allocation.Variety, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Id).ToArray();
            CropPlanFinancialEntry[] allocationFinancials = financials
                .Where(x => x.AllocationId is { } allocationId && lineageAllocationIds.Contains(allocationId)).ToArray();
            Guid[] allocationFinancialIds = allocationFinancials.Select(x => x.Id).ToArray();
            Guid[] linkedBaseTransactionIds = financeTransactions.Where(x => x.SourceType == FinanceSourceType.Planning
                && x.SourceId is { } sourceId && allocationFinancialIds.Contains(sourceId)).Select(x => x.Id).ToArray();
            FinanceTransaction[] linkedTransactions = financeTransactions.Where(x => linkedBaseTransactionIds.Contains(x.Id)
                || x.ReversalOfId is { } reversalOfId && linkedBaseTransactionIds.Contains(reversalOfId)).ToArray();
            Guid[] directCentreIds = financeCentres.Where(x =>
                    x.Type == CostCentreType.CropZone && x.TargetId == allocation.CropZoneId
                    || x.Type == CostCentreType.Field && x.TargetId == allocation.FieldId)
                .Select(x => x.Id).ToArray();
            CostAllocation[] directLedgerAllocations = financeAllocations.Where(x => directCentreIds.Contains(x.CostCentreId)
                && !planningLedgerTransactionIds.Contains(x.TransactionId)).ToArray();
            decimal actualCost = allocationFinancials.Where(x => x.EntryType == FinancialEntryType.Cost
                    && !linkedPlanningEntryIds.Contains(x.Id)).Sum(x => x.Amount)
                + linkedTransactions.Where(x => x.EntryType == FinanceEntryType.Expense).Sum(x => x.SignedAmount)
                + directLedgerAllocations.Where(x => financeTransactionById[x.TransactionId].EntryType == FinanceEntryType.Expense)
                    .Sum(x => x.Amount * (financeTransactionById[x.TransactionId].ReversalOfId is null ? 1m : -1m));
            decimal actualRevenue = allocationFinancials.Where(x => x.EntryType == FinancialEntryType.Revenue
                    && !linkedPlanningEntryIds.Contains(x.Id)).Sum(x => x.Amount)
                + linkedTransactions.Where(x => x.EntryType == FinanceEntryType.Revenue).Sum(x => x.SignedAmount)
                + directLedgerAllocations.Where(x => financeTransactionById[x.TransactionId].EntryType == FinanceEntryType.Revenue)
                    .Sum(x => x.Amount * (financeTransactionById[x.TransactionId].ReversalOfId is null ? 1m : -1m));
            decimal plannedCost = allocationBudgets.Sum(x => x.PlannedCost);
            decimal actualYield = allocation.CropZoneId is { } zoneId ? harvests.Where(x => x.CropZoneId == zoneId && x.Unit == allocation.TargetYieldUnit).Sum(x => x.Quantity) : 0;
            CropPlanOperationVarianceResponse[] operationRows = allocationPlans.GroupBy(x => x.OperationType, StringComparer.OrdinalIgnoreCase)
                .Select(group => new CropPlanOperationVarianceResponse(group.Key, group.Count(), allocationActualOps.Count(x => x.OperationType.Equals(group.Key, StringComparison.OrdinalIgnoreCase)),
                    allocationActualOps.Count(x => x.OperationType.Equals(group.Key, StringComparison.OrdinalIgnoreCase)) - group.Count())).ToArray();
            allocationRows.Add(new CropPlanAllocationVarianceResponse(allocation.Id, allocation.FieldId, allocation.CropZoneId,
                allocation.Crop, allocation.Variety, allocation.PlannedAreaHa, allocation.TargetYield, actualYield,
                actualYield - allocation.TargetYield, allocation.TargetYieldUnit, plannedCost, actualCost, actualCost - plannedCost,
                allocation.PlannedRevenue, actualRevenue, actualRevenue - allocation.PlannedRevenue,
                allocation.PlannedRevenue - plannedCost, actualRevenue - actualCost, plan.Currency, operationRows, resourceRows));
        }
        CropPlanFinancialEntry[] unallocatedFinancials = financials.Where(x => x.AllocationId == null).ToArray();
        Guid[] unallocatedFinancialIds = unallocatedFinancials.Select(x => x.Id).ToArray();
        Guid[] unallocatedBaseTransactionIds = financeTransactions.Where(x => x.SourceType == FinanceSourceType.Planning
            && x.SourceId is { } sourceId && unallocatedFinancialIds.Contains(sourceId)).Select(x => x.Id).ToArray();
        FinanceTransaction[] unallocatedLinkedTransactions = financeTransactions.Where(x => unallocatedBaseTransactionIds.Contains(x.Id)
            || x.ReversalOfId is { } reversalOfId && unallocatedBaseTransactionIds.Contains(reversalOfId)).ToArray();
        decimal unallocatedCost = unallocatedFinancials.Where(x => x.EntryType == FinancialEntryType.Cost
                && !linkedPlanningEntryIds.Contains(x.Id)).Sum(x => x.Amount)
            + unallocatedLinkedTransactions.Where(x => x.EntryType == FinanceEntryType.Expense).Sum(x => x.SignedAmount);
        decimal unallocatedRevenue = unallocatedFinancials.Where(x => x.EntryType == FinancialEntryType.Revenue
                && !linkedPlanningEntryIds.Contains(x.Id)).Sum(x => x.Amount)
            + unallocatedLinkedTransactions.Where(x => x.EntryType == FinanceEntryType.Revenue).Sum(x => x.SignedAmount);
        decimal totalPlannedCost = allocationRows.Sum(x => x.PlannedCost);
        decimal totalActualCost = allocationRows.Sum(x => x.ActualCost) + unallocatedCost;
        decimal totalPlannedRevenue = allocationRows.Sum(x => x.PlannedRevenue);
        decimal totalActualRevenue = allocationRows.Sum(x => x.ActualRevenue) + unallocatedRevenue;
        return new CropPlanVarianceResponse(plan.Id, plan.PlanningGroupId, plan.ScenarioId, plan.Name, plan.ScenarioName,
            plan.Revision, plan.Status, plan.FarmId, plan.SeasonId, plan.Currency, allocationRows.Sum(x => x.PlannedAreaHa),
            totalPlannedCost, totalActualCost, totalActualCost - totalPlannedCost,
            totalPlannedRevenue, totalActualRevenue, totalActualRevenue - totalPlannedRevenue,
            totalPlannedRevenue - totalPlannedCost, totalActualRevenue - totalActualCost,
            plannedOperations.Length, actualOperations.Length, allocationRows);
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

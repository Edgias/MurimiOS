using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
public sealed record CropPlanVarianceResponse(Guid PlanId, Guid PlanningGroupId, Guid ScenarioId,
    string Name, string ScenarioName, int Revision, CropPlanStatus Status, Guid FarmId, Guid SeasonId,
    string Currency, decimal PlannedAreaHa, decimal PlannedCost, decimal ActualCost, decimal CostVariance,
    decimal PlannedRevenue, decimal ActualRevenue, decimal RevenueVariance, decimal PlannedMargin,
    decimal ActualMargin, int PlannedOperationCount, int ActualOperationCount,
    IReadOnlyList<CropPlanAllocationVarianceResponse> Allocations);

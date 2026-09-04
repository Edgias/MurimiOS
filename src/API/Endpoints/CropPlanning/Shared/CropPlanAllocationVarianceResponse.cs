using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record CropPlanAllocationVarianceResponse(Guid AllocationId, Guid FieldId, Guid? CropZoneId,
    string Crop, string? Variety, decimal PlannedAreaHa, decimal PlannedYield, decimal ActualYield,
    decimal YieldVariance, string YieldUnit, decimal PlannedCost, decimal ActualCost, decimal CostVariance,
    decimal PlannedRevenue, decimal ActualRevenue, decimal RevenueVariance, decimal PlannedMargin,
    decimal ActualMargin, string Currency, IReadOnlyList<CropPlanOperationVarianceResponse> Operations,
    IReadOnlyList<CropPlanResourceVarianceResponse> Resources);

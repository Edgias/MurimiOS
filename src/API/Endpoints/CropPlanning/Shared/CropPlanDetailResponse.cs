using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record CropPlanDetailResponse(CropPlanSummaryResponse Plan, IReadOnlyList<CropPlanAllocationResponse> Allocations,
    IReadOnlyList<PlannedCropOperationResponse> Operations, IReadOnlyList<CropPlanResourceBudgetResponse> Resources);

using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Compare;
using CropPlanVarianceResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared.CropPlanVarianceResponse;
public sealed record CropPlanComparisonResponse(Guid PlanningGroupId, Guid FarmId, Guid SeasonId,
    IReadOnlyList<CropPlanVarianceResponse> Plans);

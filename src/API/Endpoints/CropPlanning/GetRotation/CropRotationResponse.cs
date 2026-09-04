using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetRotation;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
public sealed record CropRotationResponse(Guid PlanId, int Revision, CropPlanStatus PlanStatus,
    Guid SeasonId, string Season, DateOnly StartsOn, DateOnly EndsOn, string Crop, string? Variety,
    decimal PlannedAreaHa, decimal TargetYield, string TargetYieldUnit);

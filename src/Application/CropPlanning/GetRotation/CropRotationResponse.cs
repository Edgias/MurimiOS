using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.GetRotation;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
public sealed record CropRotationResponse(Guid PlanId, int Revision, CropPlanStatus PlanStatus,
    Guid SeasonId, string Season, DateOnly StartsOn, DateOnly EndsOn, string Crop, string? Variety,
    decimal PlannedAreaHa, decimal TargetYield, string TargetYieldUnit);

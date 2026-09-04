using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.GetPlan;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;

public sealed record CropPlanSummaryResponse(Guid Id, Guid PlanningGroupId, Guid ScenarioId, Guid FarmId,
    Guid SeasonId, string Name, string ScenarioName, string Currency, int Revision, Guid? PreviousPlanId,
    CropPlanStatus Status, decimal PlannedAreaHa, decimal PlannedCost, decimal PlannedRevenue,
    decimal PlannedMargin, DateTimeOffset DraftedAt, DateTimeOffset? SubmittedAt, DateTimeOffset? ApprovedAt,
    DateTimeOffset? RejectedAt, string? RejectionReason, DateTimeOffset? SupersededAt, Guid? SupersededByPlanId);

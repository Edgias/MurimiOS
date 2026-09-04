using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;

public sealed record CropPlanSummaryResponse(Guid Id, Guid PlanningGroupId, Guid ScenarioId, Guid FarmId,
    Guid SeasonId, string Name, string ScenarioName, string Currency, int Revision, Guid? PreviousPlanId,
    CropPlanStatus Status, decimal PlannedAreaHa, decimal PlannedCost, decimal PlannedRevenue,
    decimal PlannedMargin, DateTimeOffset DraftedAt, DateTimeOffset? SubmittedAt, DateTimeOffset? ApprovedAt,
    DateTimeOffset? RejectedAt, string? RejectionReason, DateTimeOffset? SupersededAt, Guid? SupersededByPlanId);

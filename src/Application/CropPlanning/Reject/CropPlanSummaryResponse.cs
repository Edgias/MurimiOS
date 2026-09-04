using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.Reject;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CropPlanSummaryResponse(Guid Id, Guid PlanningGroupId, Guid ScenarioId, Guid FarmId,
    Guid SeasonId, string Name, string ScenarioName, string Currency, int Revision, Guid? PreviousPlanId,
    CropPlanStatus Status, decimal PlannedAreaHa, decimal PlannedCost, decimal PlannedRevenue,
    decimal PlannedMargin, DateTimeOffset DraftedAt, DateTimeOffset? SubmittedAt, DateTimeOffset? ApprovedAt,
    DateTimeOffset? RejectedAt, string? RejectionReason, DateTimeOffset? SupersededAt, Guid? SupersededByPlanId);

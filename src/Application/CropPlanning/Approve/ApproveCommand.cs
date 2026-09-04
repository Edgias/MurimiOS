using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.Approve;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record ApproveCommand(Guid planId) : IRequest<UseCaseResult>;

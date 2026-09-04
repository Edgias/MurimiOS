using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.CreateRiskRule;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record CreateFieldRiskRuleRequest(Guid? Id, string RuleKey, int? RuleVersion, string Name,
    string? Crop, RiskMetric Metric, RiskAggregation Aggregation, RiskComparison Comparison,
    ScoutingCategory? ScoutingCategory, string? IssueCode, decimal Threshold, int LookbackHours,
    int MinimumSamples, InsightSeverity Severity, string Message,
    DateTimeOffset? EffectiveFrom, DateTimeOffset? EffectiveTo);

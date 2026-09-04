using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
public sealed record CreateFieldRiskRuleRequest(Guid? Id, string RuleKey, int? RuleVersion, string Name,
    string? Crop, RiskMetric Metric, RiskAggregation Aggregation, RiskComparison Comparison,
    ScoutingCategory? ScoutingCategory, string? IssueCode, decimal Threshold, int LookbackHours,
    int MinimumSamples, InsightSeverity Severity, string Message,
    DateTimeOffset? EffectiveFrom, DateTimeOffset? EffectiveTo);

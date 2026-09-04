using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.RiskAssessments;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
public sealed record FieldRiskAssessmentResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, Guid RuleId, int RuleVersion, DateTimeOffset AssessedAt,
    DateTimeOffset WindowStart, DateTimeOffset WindowEnd, decimal? ObservedValue, int SampleCount,
    bool Triggered, InsightSeverity Severity, string EvaluatorVersion, JsonElement InputSnapshot);

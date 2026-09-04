using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
public sealed record FieldRiskAssessmentResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, Guid RuleId, int RuleVersion, DateTimeOffset AssessedAt,
    DateTimeOffset WindowStart, DateTimeOffset WindowEnd, decimal? ObservedValue, int SampleCount,
    bool Triggered, InsightSeverity Severity, string EvaluatorVersion, JsonElement InputSnapshot);

using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.EvaluateRisks;
using FieldRiskAssessmentResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared.FieldRiskAssessmentResponse;
public sealed record EvaluateFieldRisksResponse(Guid FieldId, DateTimeOffset EvaluatedAt,
    string EvaluatorVersion, IReadOnlyList<FieldRiskAssessmentResponse> Assessments,
    int CreatedAlerts, int ResolvedAlerts);

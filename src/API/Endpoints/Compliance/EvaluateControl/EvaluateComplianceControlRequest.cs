using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.EvaluateControl;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
public sealed record EvaluateComplianceControlRequest(ComplianceEvaluationResult Result, decimal ScorePercent,
    string? Notes, IReadOnlyList<Guid>? EvidenceAttachmentIds);

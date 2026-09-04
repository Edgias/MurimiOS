using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;

using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;

public sealed record AutomatedComplianceResult(ComplianceEvaluationResult Result, decimal ScorePercent,
    string Notes, string EvidenceSnapshotJson, IReadOnlyList<Guid> EvidenceAttachmentIds);

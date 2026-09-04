using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.EvaluateControl;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
public sealed record EvaluateComplianceControlRequest(ComplianceEvaluationResult Result, decimal ScorePercent,
    string? Notes, IReadOnlyList<Guid>? EvidenceAttachmentIds);

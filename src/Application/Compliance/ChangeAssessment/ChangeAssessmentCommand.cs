using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ChangeAssessment;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingSeverity = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingSeverity;
using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed record ChangeAssessmentCommand(Guid farmId, Guid assessmentId, string command) : IRequest<UseCaseResult>;

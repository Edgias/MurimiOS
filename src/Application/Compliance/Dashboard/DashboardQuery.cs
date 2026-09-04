using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.Dashboard;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingSeverity = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingSeverity;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;
using CorrectiveActionStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.CorrectiveActionStatus;

public sealed record DashboardQuery(Guid farmId) : IRequest<UseCaseResult>;

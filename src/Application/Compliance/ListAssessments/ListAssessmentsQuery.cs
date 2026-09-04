using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ListAssessments;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;

public sealed record ListAssessmentsQuery(Guid farmId, ComplianceAssessmentStatus? status) : IRequest<UseCaseResult>;

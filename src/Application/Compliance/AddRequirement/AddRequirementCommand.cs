using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.AddRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;

public sealed record AddRequirementCommand(Guid standardId, Guid controlId, CreateComplianceRequirementRequest request) : IRequest<UseCaseResult>;

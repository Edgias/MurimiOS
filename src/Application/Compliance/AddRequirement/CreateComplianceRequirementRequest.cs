using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.AddRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
public sealed record CreateComplianceRequirementRequest(Guid? Id, string Code, string Name, ComplianceEvidenceType EvidenceType,
    bool Required, int MinimumCount, int? MaximumAgeDays, string? Instructions);

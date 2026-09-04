using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.AddRequirement;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;
public sealed record CreateComplianceRequirementRequest(Guid? Id, string Code, string Name, ComplianceEvidenceType EvidenceType,
    bool Required, int MinimumCount, int? MaximumAgeDays, string? Instructions);

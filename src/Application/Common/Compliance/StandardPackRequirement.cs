using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;

public sealed record StandardPackRequirement(string Code, string Name, ComplianceEvidenceType Type, bool Required,
    int MinimumCount, int? MaximumAgeDays, string? Instructions);

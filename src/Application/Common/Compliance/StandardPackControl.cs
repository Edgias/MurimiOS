using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
public sealed record StandardPackControl(string Code, string Title, string Description,
    ComplianceControlApplicability Applicability, decimal Weight, bool Critical, string EvaluatorKey,
    IReadOnlyList<StandardPackRequirement> Requirements);

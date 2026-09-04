using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.AddControl;
using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
public sealed record CreateComplianceControlRequest(Guid? Id, string Code, string Title, string Description,
    ComplianceControlApplicability Applicability, decimal Weight, bool Critical, string EvaluatorKey, string EvaluatorVersion, int SortOrder);

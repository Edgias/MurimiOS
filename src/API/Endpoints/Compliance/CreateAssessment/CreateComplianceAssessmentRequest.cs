using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateAssessment;
public sealed record CreateComplianceAssessmentRequest(Guid? Id, Guid StandardVersionId, string AssessmentCode,
    DateOnly ScheduledOn, string? Auditor, JsonElement? Scope);

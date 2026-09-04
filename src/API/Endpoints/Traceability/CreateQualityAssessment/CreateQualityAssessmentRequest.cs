using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateQualityAssessment;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
public sealed record CreateQualityAssessmentRequest(Guid? Id, TraceObjectType ObjectType, Guid ObjectId,
    string Metric, decimal? NumericResult, string? TextResult, string? Unit, bool Passed, DateTimeOffset AssessedAt, string? Notes);

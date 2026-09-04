using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.RecallCase;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared;
using RecallStatus = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallStatus;
public sealed record RecallResponse(Guid Id, string Reference, string Reason, RecallStatus Status,
    DateTimeOffset InitiatedAt, DateTimeOffset? ClosedAt, IEnumerable<TraceObjectReference> Sources);

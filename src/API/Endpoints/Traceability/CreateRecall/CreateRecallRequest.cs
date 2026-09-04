using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateRecall;
using TraceObjectReference = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared.TraceObjectReference;
public sealed record CreateRecallRequest(Guid? Id, string Reference, string Reason, DateTimeOffset InitiatedAt,
    IReadOnlyList<TraceObjectReference> Sources);

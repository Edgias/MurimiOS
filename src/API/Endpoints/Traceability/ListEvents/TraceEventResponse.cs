using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.ListEvents;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
public sealed record TraceEventResponse(Guid Id, TraceEventType EventType, string BusinessStep, string Disposition,
    DateTimeOffset OccurredAt, DateTimeOffset RecordedAt, string LocationType, Guid? LocationId,
    string SourceType, Guid SourceId, string? TransactionReference);

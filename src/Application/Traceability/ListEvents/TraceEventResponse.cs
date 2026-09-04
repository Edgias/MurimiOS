using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListEvents;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
public sealed record TraceEventResponse(Guid Id, TraceEventType EventType, string BusinessStep, string Disposition,
    DateTimeOffset OccurredAt, DateTimeOffset RecordedAt, string LocationType, Guid? LocationId,
    string SourceType, Guid SourceId, string? TransactionReference);

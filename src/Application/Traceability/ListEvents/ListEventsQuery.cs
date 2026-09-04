using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListEvents;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;

public sealed record ListEventsQuery(Guid farmId) : IRequest<UseCaseResult>;

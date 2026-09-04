using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.QueryEvents;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;

public sealed record QueryEventsQuery(Guid farmId, string? eventType, DateTimeOffset? GE_eventTime, DateTimeOffset? LT_eventTime, DateTimeOffset? GE_recordTime, DateTimeOffset? LT_recordTime, string? EQ_action, string? EQ_bizStep, string? EQ_disposition, string? MATCH_anyEPC, int? perPage) : IRequest<UseCaseResult>;

using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.RecallCase;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListRecalls;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using RecallStatus = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallStatus;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
public sealed record RecallResponse(Guid Id, string Reference, string Reason, RecallStatus Status,
    DateTimeOffset InitiatedAt, DateTimeOffset? ClosedAt, IEnumerable<TraceObjectReference> Sources);

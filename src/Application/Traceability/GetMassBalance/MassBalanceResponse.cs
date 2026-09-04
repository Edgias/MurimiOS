using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.GetMassBalance;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceabilityMassBalance = global::EdgIQ.MurimiOS.Domain.StorageLot.TraceabilityMassBalance;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
public sealed record MassBalanceResponse(TraceObjectType ObjectType, Guid ObjectId, decimal Original,
    decimal Consumed, decimal Available, string Unit, bool Balanced);

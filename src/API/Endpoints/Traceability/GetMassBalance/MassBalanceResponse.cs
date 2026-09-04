using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.GetMassBalance;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
public sealed record MassBalanceResponse(TraceObjectType ObjectType, Guid ObjectId, decimal Original,
    decimal Consumed, decimal Available, string Unit, bool Balanced);

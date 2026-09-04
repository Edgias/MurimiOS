using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;
public sealed record LineageEdge(Guid LinkId, Guid EventId, TraceObjectReference From, TraceObjectReference To,
    TraceRelationshipType Relationship);

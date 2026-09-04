using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.GetRecallImpact;
using DeliveryResponse = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared.DeliveryResponse;
using TraceObjectReference = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared.TraceObjectReference;
using LineageEdge = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared.LineageEdge;
public sealed record RecallImpactResponse(Guid RecallId, IEnumerable<TraceObjectReference> Objects,
    IEnumerable<LineageEdge> Edges, IEnumerable<DeliveryResponse> Deliveries);

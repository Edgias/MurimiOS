using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared;
public sealed record LineageGraph(TraceObjectReference Root, string Direction, IEnumerable<TraceObjectReference> Nodes,
    IEnumerable<LineageEdge> Edges);

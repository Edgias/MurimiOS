using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Complete;
public sealed record CompleteWorkOrderRequest(DateTimeOffset? CompletedAt, string? Notes, IReadOnlyList<WorkOrderResultRequest>? Results);

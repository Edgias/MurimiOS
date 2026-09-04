using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
public sealed record WorkOrderHistoryResponse(Guid Id, WorkOrderStatus FromStatus, WorkOrderStatus ToStatus,
    DateTimeOffset OccurredAt, string Actor, string? Reason);

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.MaterializePlan;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
public sealed record MaterializePlanRequest(IReadOnlyList<Guid>? PlannedOperationIds, WorkOrderPriority Priority = WorkOrderPriority.Normal);

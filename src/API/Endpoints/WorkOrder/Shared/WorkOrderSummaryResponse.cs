using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;

public sealed record WorkOrderSummaryResponse(Guid Id, Guid FarmId, string Number, string Title, string WorkType,
    WorkOrderPriority Priority, WorkOrderStatus Status, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd,
    Guid? FieldId, Guid? BoundaryVersionId, Guid? CropZoneId, Guid? LivestockSubjectId, Guid? CropPlanId,
    Guid? PlannedOperationId, Guid? PreviousWorkOrderId, Guid? SupersededByWorkOrderId, int ActiveAssigneeCount,
    decimal PlannedCost, uint Version);

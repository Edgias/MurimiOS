using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;

public sealed record CreateWorkOrderRequest(Guid? Id, string? Number, string Title, string WorkType,
    WorkOrderPriority Priority, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string Instructions,
    Guid? FieldId = null, Guid? BoundaryVersionId = null, Guid? CropZoneId = null, Guid? LivestockSubjectId = null,
    Guid? CropPlanId = null, Guid? PlannedOperationId = null);

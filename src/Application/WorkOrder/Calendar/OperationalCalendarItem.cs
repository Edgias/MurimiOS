using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Calendar;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
public sealed record OperationalCalendarItem(string Source, Guid Id, string? Number, string Title, string WorkType,
    string Status, DateTimeOffset Start, DateTimeOffset End, Guid? FieldId, Guid? CropZoneId,
    Guid? LivestockSubjectId, Guid? CropPlanId, Guid? PlannedOperationId, bool RequiresMaterialization);

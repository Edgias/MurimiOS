using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateFollowUpWorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
public sealed record CreateAlertWorkOrderRequest(Guid? Id, string? Number, string Title, string WorkType,
    WorkOrderPriority Priority, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd,
    string Instructions, bool PlanImmediately);

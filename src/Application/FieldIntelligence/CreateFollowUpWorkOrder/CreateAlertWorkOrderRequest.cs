using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.CreateFollowUpWorkOrder;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
public sealed record CreateAlertWorkOrderRequest(Guid? Id, string? Number, string Title, string WorkType,
    WorkOrderPriority Priority, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd,
    string Instructions, bool PlanImmediately);

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Assign;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using NotificationSeverity = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationSeverity;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderAssignment = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderAssignment;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;
using WorkResultType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResultType;
public sealed record WorkOrderHistoryResponse(Guid Id, WorkOrderStatus FromStatus, WorkOrderStatus ToStatus,
    DateTimeOffset OccurredAt, string Actor, string? Reason);

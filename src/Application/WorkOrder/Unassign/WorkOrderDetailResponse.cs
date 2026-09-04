using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Unassign;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
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
public sealed record WorkOrderDetailResponse(WorkOrderSummaryResponse WorkOrder, string Instructions, string CreatedBy,
    DateTimeOffset DraftedAt, DateTimeOffset? PlannedAt, DateTimeOffset? AssignedAt, DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt, DateTimeOffset? VerifiedAt, string? VerifiedBy, DateTimeOffset? RejectedAt,
    string? RejectedBy, string? RejectionReason, DateTimeOffset? CancelledAt, string? CancelledBy,
    string? CancellationReason, string? CompletionNotes, IReadOnlyList<WorkOrderAssignmentResponse> Assignments,
    IReadOnlyList<WorkOrderResourceResponse> Resources, IReadOnlyList<WorkOrderHistoryResponse> History,
    IReadOnlyList<WorkOrderResultResponse> Results);

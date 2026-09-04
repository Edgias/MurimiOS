using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
public sealed record WorkOrderDetailResponse(WorkOrderSummaryResponse WorkOrder, string Instructions, string CreatedBy,
    DateTimeOffset DraftedAt, DateTimeOffset? PlannedAt, DateTimeOffset? AssignedAt, DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt, DateTimeOffset? VerifiedAt, string? VerifiedBy, DateTimeOffset? RejectedAt,
    string? RejectedBy, string? RejectionReason, DateTimeOffset? CancelledAt, string? CancelledBy,
    string? CancellationReason, string? CompletionNotes, IReadOnlyList<WorkOrderAssignmentResponse> Assignments,
    IReadOnlyList<WorkOrderResourceResponse> Resources, IReadOnlyList<WorkOrderHistoryResponse> History,
    IReadOnlyList<WorkOrderResultResponse> Results);

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public sealed class WorkOrderStatusHistory : TenantEntity
{
    private WorkOrderStatusHistory() { }
    public WorkOrderStatusHistory(Guid id, Guid workOrderId, WorkOrderStatus from, WorkOrderStatus to,
        DateTimeOffset occurredAt, string actor, string? reason) : base(id)
    {
        if (workOrderId == Guid.Empty || !Enum.IsDefined(from) || !Enum.IsDefined(to) || from == to) throw new ArgumentException("A valid work-order transition is required.");
        WorkOrderId = workOrderId; FromStatus = from; ToStatus = to; OccurredAt = occurredAt.ToUniversalTime();
        Actor = Text(actor, nameof(actor), 240); Reason = Optional(reason, 2_000);
    }
    public Guid WorkOrderId { get; private set; }
    public WorkOrderStatus FromStatus { get; private set; }
    public WorkOrderStatus ToStatus { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Actor { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

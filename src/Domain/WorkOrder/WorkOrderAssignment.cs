using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public sealed class WorkOrderAssignment : TenantEntity
{
    private WorkOrderAssignment() { }
    public WorkOrderAssignment(Guid id, Guid workOrderId, Guid membershipId, string assignmentRole,
        bool isPrimary, DateTimeOffset assignedAt, string assignedBy) : base(id)
    {
        if (workOrderId == Guid.Empty || membershipId == Guid.Empty) throw new ArgumentException("Work order and membership are required.");
        WorkOrderId = workOrderId; MembershipId = membershipId;
        AssignmentRole = Text(assignmentRole, nameof(assignmentRole), 100); IsPrimary = isPrimary;
        AssignedAt = assignedAt.ToUniversalTime(); AssignedBy = Text(assignedBy, nameof(assignedBy), 240);
    }
    public Guid WorkOrderId { get; private set; }
    public Guid MembershipId { get; private set; }
    public string AssignmentRole { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public string AssignedBy { get; private set; } = string.Empty;
    public DateTimeOffset? UnassignedAt { get; private set; }
    public string? UnassignedBy { get; private set; }
    public void Unassign(DateTimeOffset at, string actor)
    {
        if (UnassignedAt is not null) throw new InvalidOperationException("The assignment has already ended.");
        UnassignedAt = at.ToUniversalTime(); UnassignedBy = Text(actor, nameof(actor), 240);
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

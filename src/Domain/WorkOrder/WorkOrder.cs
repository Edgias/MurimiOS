using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public sealed class WorkOrder : TenantEntity
{
    private WorkOrder() { }

    public WorkOrder(Guid id, Guid farmId, string number, string title, string workType,
        WorkOrderPriority priority, DateTimeOffset scheduledStart, DateTimeOffset scheduledEnd,
        string instructions, string createdBy, DateTimeOffset createdAt,
        Guid? fieldId = null, Guid? boundaryVersionId = null, Guid? cropZoneId = null,
        Guid? livestockSubjectId = null, Guid? cropPlanId = null, Guid? plannedOperationId = null,
        Guid? previousWorkOrderId = null) : base(id)
    {
        ValidateIdentifiers(farmId, fieldId, boundaryVersionId, cropZoneId, livestockSubjectId,
            cropPlanId, plannedOperationId, previousWorkOrderId);
        ValidateSchedule(scheduledStart, scheduledEnd);
        if (!Enum.IsDefined(priority)) throw new ArgumentOutOfRangeException(nameof(priority));
        FarmId = farmId;
        Number = Text(number, nameof(number), 80);
        Title = Text(title, nameof(title), 200);
        WorkType = Text(workType, nameof(workType), 80);
        Priority = priority;
        ScheduledStart = scheduledStart.ToUniversalTime();
        ScheduledEnd = scheduledEnd.ToUniversalTime();
        Instructions = Text(instructions, nameof(instructions), 4_000);
        FieldId = fieldId;
        BoundaryVersionId = boundaryVersionId;
        CropZoneId = cropZoneId;
        LivestockSubjectId = livestockSubjectId;
        CropPlanId = cropPlanId;
        PlannedOperationId = plannedOperationId;
        PreviousWorkOrderId = previousWorkOrderId;
        CreatedBy = Text(createdBy, nameof(createdBy), 240);
        DraftedAt = createdAt.ToUniversalTime();
        Status = WorkOrderStatus.Draft;
    }

    public Guid FarmId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string WorkType { get; private set; } = string.Empty;
    public WorkOrderPriority Priority { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public DateTimeOffset ScheduledStart { get; private set; }
    public DateTimeOffset ScheduledEnd { get; private set; }
    public string Instructions { get; private set; } = string.Empty;
    public Guid? FieldId { get; private set; }
    public Guid? BoundaryVersionId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public Guid? LivestockSubjectId { get; private set; }
    public Guid? CropPlanId { get; private set; }
    public Guid? PlannedOperationId { get; private set; }
    public Guid? PreviousWorkOrderId { get; private set; }
    public Guid? SupersededByWorkOrderId { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset DraftedAt { get; private set; }
    public DateTimeOffset? PlannedAt { get; private set; }
    public DateTimeOffset? AssignedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public string? RejectedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? CompletionNotes { get; private set; }

    public void UpdateDraft(string title, string workType, WorkOrderPriority priority,
        DateTimeOffset scheduledStart, DateTimeOffset scheduledEnd, string instructions)
    {
        if (Status is not (WorkOrderStatus.Draft or WorkOrderStatus.Planned))
            throw new InvalidOperationException("Only draft or planned work orders can be edited.");
        ValidateSchedule(scheduledStart, scheduledEnd);
        if (!Enum.IsDefined(priority)) throw new ArgumentOutOfRangeException(nameof(priority));
        Title = Text(title, nameof(title), 200);
        WorkType = Text(workType, nameof(workType), 80);
        Priority = priority;
        ScheduledStart = scheduledStart.ToUniversalTime();
        ScheduledEnd = scheduledEnd.ToUniversalTime();
        Instructions = Text(instructions, nameof(instructions), 4_000);
    }

    public void Plan(DateTimeOffset at) => Transition(WorkOrderStatus.Draft, WorkOrderStatus.Planned, () => PlannedAt = at.ToUniversalTime());
    public void MarkAssigned(DateTimeOffset at)
    {
        if (Status == WorkOrderStatus.Planned)
            Transition(WorkOrderStatus.Planned, WorkOrderStatus.Assigned, () => AssignedAt = at.ToUniversalTime());
        else if (Status != WorkOrderStatus.Assigned)
            throw new InvalidOperationException("Only a planned work order can be assigned.");
    }
    public void ReturnToPlanned()
    {
        if (Status != WorkOrderStatus.Assigned) throw new InvalidOperationException("Only an assigned work order can return to planned.");
        Status = WorkOrderStatus.Planned;
        AssignedAt = null;
    }
    public void Start(DateTimeOffset at) => Transition(WorkOrderStatus.Assigned, WorkOrderStatus.InProgress, () => StartedAt = at.ToUniversalTime());
    public void Complete(DateTimeOffset at, string? notes) => Transition(WorkOrderStatus.InProgress, WorkOrderStatus.Completed, () =>
    {
        CompletedAt = at.ToUniversalTime();
        CompletionNotes = Optional(notes, 4_000);
    });
    public void Verify(DateTimeOffset at, string actor) => Transition(WorkOrderStatus.Completed, WorkOrderStatus.Verified, () =>
    {
        VerifiedAt = at.ToUniversalTime();
        VerifiedBy = Text(actor, nameof(actor), 240);
    });
    public void Reject(DateTimeOffset at, string actor, string reason) => Transition(WorkOrderStatus.Completed, WorkOrderStatus.Rejected, () =>
    {
        RejectedAt = at.ToUniversalTime();
        RejectedBy = Text(actor, nameof(actor), 240);
        RejectionReason = Text(reason, nameof(reason), 2_000);
    });
    public void Reopen(DateTimeOffset at)
    {
        Transition(WorkOrderStatus.Rejected, WorkOrderStatus.Assigned, () =>
        {
            AssignedAt = at.ToUniversalTime();
            RejectionReason = null;
        });
    }
    public void Cancel(DateTimeOffset at, string actor, string reason)
    {
        if (Status is not (WorkOrderStatus.Draft or WorkOrderStatus.Planned or WorkOrderStatus.Assigned or WorkOrderStatus.Rejected))
            throw new InvalidOperationException("Only draft, planned, assigned, or rejected work orders can be cancelled.");
        Status = WorkOrderStatus.Cancelled;
        CancelledAt = at.ToUniversalTime();
        CancelledBy = Text(actor, nameof(actor), 240);
        CancellationReason = Text(reason, nameof(reason), 2_000);
    }
    public void Supersede(Guid replacementId)
    {
        if (Status != WorkOrderStatus.Verified) throw new InvalidOperationException("Only a verified work order can be corrected.");
        if (replacementId == Guid.Empty || replacementId == Id) throw new ArgumentException("A distinct replacement is required.", nameof(replacementId));
        Status = WorkOrderStatus.Superseded;
        SupersededByWorkOrderId = replacementId;
    }

    private void Transition(WorkOrderStatus expected, WorkOrderStatus next, Action update)
    {
        if (Status != expected) throw new InvalidOperationException($"Work order must be {expected} before it can become {next}.");
        Status = next;
        update();
    }
    private static void ValidateSchedule(DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start) throw new ArgumentException("Scheduled end must be later than scheduled start.");
    }
    private static void ValidateIdentifiers(Guid farmId, Guid? fieldId, Guid? boundaryVersionId, Guid? cropZoneId,
        Guid? livestockSubjectId, Guid? cropPlanId, Guid? plannedOperationId, Guid? previousWorkOrderId)
    {
        if (farmId == Guid.Empty || fieldId == Guid.Empty || boundaryVersionId == Guid.Empty || cropZoneId == Guid.Empty
            || livestockSubjectId == Guid.Empty || cropPlanId == Guid.Empty || plannedOperationId == Guid.Empty || previousWorkOrderId == Guid.Empty)
            throw new ArgumentException("Farm is required and optional identifiers must be non-empty.");
        if (fieldId is null && (boundaryVersionId is not null || cropZoneId is not null))
            throw new ArgumentException("Boundary and crop-zone targets require a field.");
        if (fieldId is not null && boundaryVersionId is null)
            throw new ArgumentException("A field work order must preserve its boundary version.");
        if (fieldId is not null && livestockSubjectId is not null)
            throw new ArgumentException("A work order cannot target both a field and livestock.");
        if ((cropPlanId is null) != (plannedOperationId is null))
            throw new ArgumentException("Crop plan and planned operation identifiers must be supplied together.");
    }
    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

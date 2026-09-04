using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropPlan;

public sealed class PlannedCropOperation : TenantEntity
{
    private PlannedCropOperation() { }
    public PlannedCropOperation(Guid id, Guid planId, Guid allocationId, string operationType,
        DateOnly plannedStart, DateOnly plannedEnd, int sequence, string? notes) : base(id)
    {
        if (planId == Guid.Empty || allocationId == Guid.Empty) throw new ArgumentException("Plan and allocation are required.");
        if (plannedEnd < plannedStart) throw new ArgumentException("Planned operation end cannot precede its start.");
        if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
        PlanId = planId; AllocationId = allocationId;
        OperationType = Text(operationType, nameof(operationType), 80);
        PlannedStart = plannedStart; PlannedEnd = plannedEnd; Sequence = sequence;
        Notes = Optional(notes, 2_000);
    }
    public Guid PlanId { get; private set; }
    public Guid AllocationId { get; private set; }
    public string OperationType { get; private set; } = string.Empty;
    public DateOnly PlannedStart { get; private set; }
    public DateOnly PlannedEnd { get; private set; }
    public int Sequence { get; private set; }
    public string? Notes { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

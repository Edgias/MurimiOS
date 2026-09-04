using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockRationPlan;

public sealed class LivestockRationPlan : TenantEntity
{
    private LivestockRationPlan() { }
    public LivestockRationPlan(Guid id, Guid farmId, Guid subjectId, string name, DateOnly effectiveFrom,
        DateOnly? effectiveTo, int targetHeadCount, string? notes, string createdBy) : base(id)
    {
        if (farmId == Guid.Empty || subjectId == Guid.Empty) throw new ArgumentException("Farm and subject are required.");
        if (effectiveTo < effectiveFrom || targetHeadCount <= 0) throw new ArgumentOutOfRangeException(nameof(effectiveTo));
        FarmId = farmId; SubjectId = subjectId; Name = Text(name, 200); EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo;
        TargetHeadCount = targetHeadCount; Notes = Optional(notes, 2_000); CreatedBy = Text(createdBy, 240); Status = RationPlanStatus.Draft;
    }
    public Guid FarmId { get; private set; }
    public Guid SubjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public int TargetHeadCount { get; private set; }
    public string? Notes { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public RationPlanStatus Status { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Activate(DateTimeOffset at) { if (Status != RationPlanStatus.Draft) throw new InvalidOperationException("Only a draft ration can be activated."); Status = RationPlanStatus.Active; ActivatedAt = at; }
    public void Retire(DateTimeOffset at) { if (Status != RationPlanStatus.Active) throw new InvalidOperationException("Only an active ration can be retired."); Status = RationPlanStatus.Retired; RetiredAt = at; }
    private static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

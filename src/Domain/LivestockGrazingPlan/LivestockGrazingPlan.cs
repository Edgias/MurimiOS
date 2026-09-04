using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;

public sealed class LivestockGrazingPlan : TenantEntity
{
    private LivestockGrazingPlan() { }
    public LivestockGrazingPlan(Guid id, Guid farmId, Guid groupSubjectId, Guid holdingId, DateTimeOffset startsAt,
        DateTimeOffset endsAt, int targetHeadCount, DateTimeOffset? restUntil, decimal? availableDryMatterKg,
        string? notes, string createdBy) : base(id)
    {
        if (farmId == Guid.Empty || groupSubjectId == Guid.Empty || holdingId == Guid.Empty) throw new ArgumentException("Farm, group and holding are required.");
        if (endsAt <= startsAt || targetHeadCount <= 0 || restUntil < endsAt || availableDryMatterKg < 0) throw new ArgumentOutOfRangeException(nameof(endsAt));
        FarmId = farmId; GroupSubjectId = groupSubjectId; HoldingId = holdingId; StartsAt = startsAt.ToUniversalTime(); EndsAt = endsAt.ToUniversalTime();
        TargetHeadCount = targetHeadCount; RestUntil = restUntil?.ToUniversalTime(); AvailableDryMatterKg = availableDryMatterKg;
        Notes = Optional(notes, 2_000); CreatedBy = Text(createdBy, 240); Status = GrazingPlanStatus.Draft;
    }
    public Guid FarmId { get; private set; }
    public Guid GroupSubjectId { get; private set; }
    public Guid HoldingId { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public int TargetHeadCount { get; private set; }
    public DateTimeOffset? RestUntil { get; private set; }
    public decimal? AvailableDryMatterKg { get; private set; }
    public string? Notes { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public GrazingPlanStatus Status { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public void Activate() { if (Status != GrazingPlanStatus.Draft) throw new InvalidOperationException("Only a draft grazing plan can be activated."); Status = GrazingPlanStatus.Active; }
    public void Complete(DateTimeOffset at) { if (Status != GrazingPlanStatus.Active) throw new InvalidOperationException("Only an active grazing plan can be completed."); Status = GrazingPlanStatus.Completed; ClosedAt = at; }
    public void Cancel(DateTimeOffset at) { if (Status is GrazingPlanStatus.Completed or GrazingPlanStatus.Cancelled) throw new InvalidOperationException("The grazing plan is already closed."); Status = GrazingPlanStatus.Cancelled; ClosedAt = at; }
    private static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException("A valid actor is required."); return result; }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

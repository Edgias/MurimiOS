using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropPlan;

public sealed class CropPlan : TenantEntity
{
    private CropPlan() { }

    public CropPlan(Guid id, Guid planningGroupId, Guid scenarioId, Guid farmId, Guid seasonId,
        string name, string scenarioName, string currency, int revision, Guid? previousPlanId,
        DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (planningGroupId == Guid.Empty || scenarioId == Guid.Empty || farmId == Guid.Empty || seasonId == Guid.Empty)
            throw new ArgumentException("Planning group, scenario, farm, and season identifiers are required.");
        if (revision <= 0 || previousPlanId == Guid.Empty)
            throw new ArgumentException("Revision must be positive and a previous plan id must be non-empty when supplied.");
        PlanningGroupId = planningGroupId;
        ScenarioId = scenarioId;
        FarmId = farmId;
        SeasonId = seasonId;
        Name = Text(name, nameof(name), 200);
        ScenarioName = Text(scenarioName, nameof(scenarioName), 120);
        Currency = new CurrencyCode(currency).Value;
        Revision = revision;
        PreviousPlanId = previousPlanId;
        Status = CropPlanStatus.Draft;
        CreatedBy = Text(createdBy, nameof(createdBy), 240);
        DraftedAt = createdAt.ToUniversalTime();
    }

    public Guid PlanningGroupId { get; private set; }
    public Guid ScenarioId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid SeasonId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ScenarioName { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public int Revision { get; private set; }
    public Guid? PreviousPlanId { get; private set; }
    public CropPlanStatus Status { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset DraftedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public string? SubmittedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public string? RejectedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset? SupersededAt { get; private set; }
    public Guid? SupersededByPlanId { get; private set; }

    public void EnsureDraft()
    {
        if (Status != CropPlanStatus.Draft)
            throw new InvalidOperationException("Only a draft crop plan can be edited.");
    }

    public void Submit(DateTimeOffset at, string actor)
    {
        EnsureDraft();
        Status = CropPlanStatus.Submitted;
        SubmittedAt = at.ToUniversalTime();
        SubmittedBy = Text(actor, nameof(actor), 240);
    }

    public void Approve(DateTimeOffset at, string actor)
    {
        if (Status != CropPlanStatus.Submitted)
            throw new InvalidOperationException("Only a submitted crop plan can be approved.");
        Status = CropPlanStatus.Approved;
        ApprovedAt = at.ToUniversalTime();
        ApprovedBy = Text(actor, nameof(actor), 240);
    }

    public void Reject(DateTimeOffset at, string actor, string reason)
    {
        if (Status != CropPlanStatus.Submitted)
            throw new InvalidOperationException("Only a submitted crop plan can be rejected.");
        Status = CropPlanStatus.Rejected;
        RejectedAt = at.ToUniversalTime();
        RejectedBy = Text(actor, nameof(actor), 240);
        RejectionReason = Text(reason, nameof(reason), 2_000);
    }

    public void Supersede(Guid replacementPlanId, DateTimeOffset at)
    {
        if (Status != CropPlanStatus.Approved)
            throw new InvalidOperationException("Only an approved crop plan can be superseded.");
        if (replacementPlanId == Guid.Empty || replacementPlanId == Id)
            throw new ArgumentException("A distinct replacement plan is required.", nameof(replacementPlanId));
        Status = CropPlanStatus.Superseded;
        SupersededByPlanId = replacementPlanId;
        SupersededAt = at.ToUniversalTime();
    }

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
}

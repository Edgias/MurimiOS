using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.KpiDefinition;

public sealed class KpiDefinition : TenantEntity
{
    private KpiDefinition() { }
    public KpiDefinition(Guid id, Guid seriesId, string code, string name, string description, string formula,
        string unit, string timeGrain, string owner, string exclusions, string qualityRule, int version) : base(id)
    {
        if (seriesId == Guid.Empty || version <= 0) throw new ArgumentException("A KPI series and positive version are required.");
        SeriesId = seriesId; Code = Text(code, 120).ToLowerInvariant(); Name = Text(name, 200); Description = Text(description, 2_000);
        Formula = Text(formula, 4_000); Unit = Text(unit, 40); TimeGrain = Text(timeGrain, 40); Owner = Text(owner, 240);
        Exclusions = Text(exclusions, 2_000); QualityRule = Text(qualityRule, 2_000); DefinitionVersion = version; Status = KpiDefinitionStatus.Draft;
    }
    public Guid SeriesId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Formula { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public string TimeGrain { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public string Exclusions { get; private set; } = string.Empty;
    public string QualityRule { get; private set; } = string.Empty;
    public int DefinitionVersion { get; private set; }
    public KpiDefinitionStatus Status { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Activate(DateTimeOffset at) { if (Status != KpiDefinitionStatus.Draft) throw new InvalidOperationException("Only a draft KPI definition can activate."); Status = KpiDefinitionStatus.Active; ActivatedAt = at.ToUniversalTime(); }
    public void Retire(DateTimeOffset at) { if (Status != KpiDefinitionStatus.Active) throw new InvalidOperationException("Only an active KPI definition can retire."); Status = KpiDefinitionStatus.Retired; RetiredAt = at.ToUniversalTime(); }
    internal static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
}



namespace EdgIQ.MurimiOS.Domain.DataGrant;

public static class SupplyDataCategories
{
    public const string HarvestLots = "harvest-lots";
    public const string Traceability = "traceability";
    public const string QualityAssessments = "quality-assessments";
    public const string Evidence = "evidence";
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        HarvestLots, Traceability, QualityAssessments, Evidence
    };
}

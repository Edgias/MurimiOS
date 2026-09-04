using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;

public sealed class EnterpriseConsolidationRun : TenantEntity
{
    private EnterpriseConsolidationRun() { }

    public EnterpriseConsolidationRun(Guid id, Guid organisationNodeId, DateTimeOffset periodStart,
        DateTimeOffset periodEnd, string currency, decimal grossExpenses, decimal grossRevenue,
        decimal eliminatedExpenses, decimal eliminatedRevenue, int farmCount, int companyCount,
        int sourceTransactionCount, int exceptionCount, string sourceSha256, DateTimeOffset completedAt,
        string requestedBy) : base(id)
    {
        if (organisationNodeId == Guid.Empty || periodEnd <= periodStart || farmCount < 0 || companyCount < 1
            || sourceTransactionCount < 0 || exceptionCount < 0 || eliminatedExpenses < 0 || eliminatedRevenue < 0)
            throw new ArgumentException("The consolidation scope, period, counts, and elimination totals must be valid.");
        OrganisationNodeId = organisationNodeId; PeriodStart = periodStart.ToUniversalTime(); PeriodEnd = periodEnd.ToUniversalTime();
        Currency = new CurrencyCode(currency).Value; GrossExpenses = grossExpenses; GrossRevenue = grossRevenue;
        EliminatedExpenses = eliminatedExpenses; EliminatedRevenue = eliminatedRevenue;
        ConsolidatedExpenses = grossExpenses - eliminatedExpenses; ConsolidatedRevenue = grossRevenue - eliminatedRevenue;
        ConsolidatedMargin = ConsolidatedRevenue - ConsolidatedExpenses; FarmCount = farmCount; CompanyCount = companyCount;
        SourceTransactionCount = sourceTransactionCount; ExceptionCount = exceptionCount;
        SourceSha256 = LegalCompany.Required(sourceSha256, 64).ToLowerInvariant();
        if (SourceSha256.Length != 64 || SourceSha256.Any(c => !Uri.IsHexDigit(c))) throw new ArgumentException("A SHA-256 source digest is required.");
        CompletedAt = completedAt.ToUniversalTime(); RequestedBy = LegalCompany.Required(requestedBy, 240);
    }

    public Guid OrganisationNodeId { get; private set; }
    public DateTimeOffset PeriodStart { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal GrossExpenses { get; private set; }
    public decimal GrossRevenue { get; private set; }
    public decimal EliminatedExpenses { get; private set; }
    public decimal EliminatedRevenue { get; private set; }
    public decimal ConsolidatedExpenses { get; private set; }
    public decimal ConsolidatedRevenue { get; private set; }
    public decimal ConsolidatedMargin { get; private set; }
    public int FarmCount { get; private set; }
    public int CompanyCount { get; private set; }
    public int SourceTransactionCount { get; private set; }
    public int ExceptionCount { get; private set; }
    public string SourceSha256 { get; private set; } = string.Empty;
    public DateTimeOffset CompletedAt { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
}

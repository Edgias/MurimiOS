using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Finance;
using ConsolidationEntryStatus = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.ConsolidationEntryStatus;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using EliminationRuleStatus = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.EliminationRuleStatus;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;

public sealed class EnterpriseConsolidationEngine
{
    public async Task<EnterpriseConsolidationBuild> BuildAsync(Guid runId, Guid organisationNodeId,
        DateTimeOffset periodStart, DateTimeOffset periodEnd, string currency, string requestedBy,
        DateTimeOffset completedAt, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        if (periodEnd <= periodStart) throw new ArgumentException("The consolidation period end must follow its start.");
        OrganisationNode root = await db.OrganisationNodes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == organisationNodeId, cancellationToken)
            ?? throw new ArgumentException("The consolidation organisation node was not found.");
        OrganisationNode[] nodes = await db.OrganisationNodes.AsNoTracking().ToArrayAsync(cancellationToken);
        OrganisationNode[] descendants = nodes.Where(x => IsDescendant(x.Path, root.Path)).ToArray();
        HashSet<Guid> descendantIds = descendants.Select(x => x.Id).ToHashSet();
        var nodeById = nodes.ToDictionary(x => x.Id);
        Guid[] farmIds = await db.Farms.AsNoTracking().Where(x => descendantIds.Contains(x.OrganisationNodeId))
            .Select(x => x.Id).ToArrayAsync(cancellationToken);
        var farmNodes = await db.Farms.AsNoTracking().Where(x => farmIds.Contains(x.Id))
            .Select(x => new { x.Id, x.OrganisationNodeId }).ToArrayAsync(cancellationToken);
        DateOnly fromDate = DateOnly.FromDateTime(periodStart.UtcDateTime);
        DateOnly toDate = DateOnly.FromDateTime(periodEnd.AddTicks(-1).UtcDateTime);
        LegalCompany[] companies = await db.LegalCompanies.AsNoTracking()
            .Where(x => descendantIds.Contains(x.OrganisationNodeId) && x.EffectiveFrom <= toDate
                && (x.EffectiveTo == null || x.EffectiveTo >= fromDate)).ToArrayAsync(cancellationToken);
        if (companies.Length == 0) throw new InvalidOperationException("At least one legal company must be registered in the consolidation scope.");
        var companyPaths = companies.Select(x => new { Company = x, Path = nodeById[x.OrganisationNodeId].Path })
            .OrderByDescending(x => x.Path.Length).ToArray();
        Dictionary<Guid, Guid> owningCompany = farmNodes.Select(x => new
            {
                x.Id,
                CompanyId = companyPaths.FirstOrDefault(company => IsDescendant(nodeById[x.OrganisationNodeId].Path, company.Path))?.Company.Id
            }).Where(x => x.CompanyId is not null).ToDictionary(x => x.Id, x => x.CompanyId!.Value);
        if (owningCompany.Count != farmIds.Length)
            throw new InvalidOperationException("Every farm in the consolidation scope must belong to an effective legal company.");

        FinanceTransaction[] transactions = await db.FinanceTransactions.AsNoTracking()
            .Where(x => farmIds.Contains(x.FarmId) && x.Currency == currency && x.OccurredAt >= periodStart && x.OccurredAt < periodEnd)
            .OrderBy(x => x.Id).ToArrayAsync(cancellationToken);
        IntercompanyEliminationRule[] rules = await db.IntercompanyEliminationRules.AsNoTracking()
            .Where(x => x.ConsolidationOrganisationNodeId == organisationNodeId && x.Currency == currency
                && x.Status == EliminationRuleStatus.Active && x.EffectiveFrom <= toDate
                && (x.EffectiveTo == null || x.EffectiveTo >= fromDate))
            .OrderBy(x => x.RuleKey).ThenBy(x => x.RuleVersion).ToArrayAsync(cancellationToken);
        var entries = new List<EnterpriseEliminationEntry>();
        foreach (IntercompanyEliminationRule rule in rules)
        {
            FinanceTransaction[] seller = transactions.Where(x => owningCompany.GetValueOrDefault(x.FarmId) == rule.SellerCompanyId
                && x.CounterpartyLegalCompanyId == rule.BuyerCompanyId && x.EntryType == FinanceEntryType.Revenue
                && x.IntercompanyReference is not null).ToArray();
            FinanceTransaction[] buyer = transactions.Where(x => owningCompany.GetValueOrDefault(x.FarmId) == rule.BuyerCompanyId
                && x.CounterpartyLegalCompanyId == rule.SellerCompanyId && x.EntryType == FinanceEntryType.Expense
                && x.IntercompanyReference is not null).ToArray();
            string[] references = seller.Select(x => x.IntercompanyReference!).Concat(buyer.Select(x => x.IntercompanyReference!))
                .Distinct(StringComparer.OrdinalIgnoreCase).Order().ToArray();
            foreach (string reference in references)
            {
                FinanceTransaction[] sellerRows = seller.Where(x => x.IntercompanyReference!.Equals(reference, StringComparison.OrdinalIgnoreCase)).ToArray();
                FinanceTransaction[] buyerRows = buyer.Where(x => x.IntercompanyReference!.Equals(reference, StringComparison.OrdinalIgnoreCase)).ToArray();
                decimal sellerRevenue = Math.Max(0, sellerRows.Sum(x => x.SignedAmount));
                decimal buyerExpense = Math.Max(0, buyerRows.Sum(x => x.SignedAmount));
                decimal eliminated = Math.Min(sellerRevenue, buyerExpense);
                entries.Add(new EnterpriseEliminationEntry(Guid.NewGuid(), runId, rule.Id, reference,
                    sellerRevenue, buyerExpense, eliminated, sellerRevenue - buyerExpense, rule.BalancingTolerance,
                    JsonSerializer.Serialize(sellerRows.Select(x => x.Id).Order()),
                    JsonSerializer.Serialize(buyerRows.Select(x => x.Id).Order())));
            }
        }

        decimal grossExpenses = transactions.Where(x => x.EntryType == FinanceEntryType.Expense).Sum(x => x.SignedAmount);
        decimal grossRevenue = transactions.Where(x => x.EntryType == FinanceEntryType.Revenue).Sum(x => x.SignedAmount);
        decimal eliminatedExpenses = entries.Sum(x => x.EliminatedAmount);
        decimal eliminatedRevenue = entries.Sum(x => x.EliminatedAmount);
        string source = JsonSerializer.Serialize(new
        {
            organisationNodeId, periodStart, periodEnd, currency,
            companies = companies.Select(x => new { x.Id, x.Version }).OrderBy(x => x.Id),
            rules = rules.Select(x => new { x.Id, x.RuleVersion, x.Version }).OrderBy(x => x.Id),
            transactions = transactions.Select(x => new { x.Id, x.Version, x.Amount, x.EntryType, x.ReversalOfId,
                x.CounterpartyLegalCompanyId, x.IntercompanyReference }).OrderBy(x => x.Id)
        });
        string sha = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source))).ToLowerInvariant();
        var run = new EnterpriseConsolidationRun(runId, organisationNodeId, periodStart, periodEnd, currency,
            grossExpenses, grossRevenue, eliminatedExpenses, eliminatedRevenue, farmIds.Length, companies.Length,
            transactions.Length, entries.Count(x => x.Status == ConsolidationEntryStatus.Exception), sha, completedAt, requestedBy);
        return new EnterpriseConsolidationBuild(run, entries.ToArray(), farmIds, companies.Select(x => x.Id).ToArray());
    }

    private static bool IsDescendant(string candidate, string root) => candidate.Equals(root, StringComparison.Ordinal)
        || candidate.StartsWith(root + ".", StringComparison.Ordinal);
}

using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.GetEnterpriseProfitability;

using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class GetEnterpriseProfitabilityHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetEnterpriseProfitabilityQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetEnterpriseProfitabilityQuery message, CancellationToken cancellationToken) => GetEnterpriseProfitabilityAsync(message.organisationNodeId, message.from, message.to, message.currency, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetEnterpriseProfitabilityAsync(Guid organisationNodeId,
        DateTimeOffset from, DateTimeOffset to, string currency, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (to <= from) return Bad("profitability_period_invalid", "The period end must follow its start.");
        string normalizedCurrency;
        try { normalizedCurrency = new CurrencyCode(currency).Value; }
        catch (ArgumentException ex) { return Bad("currency_invalid", ex.Message); }
        ResolvedOrganisationScope callerScope = await scope.ResolveAsync(cancellationToken);
        if (!callerScope.AllowsNode(organisationNodeId)) return UseCaseResults.NotFound();
        string? rootPath = await db.OrganisationNodes.AsNoTracking().Where(x => x.Id == organisationNodeId)
            .Select(x => x.Path).SingleOrDefaultAsync(cancellationToken);
        if (rootPath is null) return UseCaseResults.NotFound();
        Guid[] nodeIds;
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
        {
            nodeIds = await db.OrganisationNodes.FromSqlInterpolated($$"""
                SELECT child.* FROM platform.org_nodes AS child
                JOIN platform.org_nodes AS root ON root.tenant_id = child.tenant_id AND root.id = {{organisationNodeId}}
                WHERE child.path <@ root.path
                """).AsNoTracking().Select(x => x.Id).ToArrayAsync(cancellationToken);
        }
        else
        {
            var nodes = await db.OrganisationNodes.AsNoTracking().Select(x => new { x.Id, x.Path }).ToArrayAsync(cancellationToken);
            nodeIds = nodes.Where(x => x.Path == rootPath || x.Path.StartsWith(rootPath + ".", StringComparison.Ordinal))
                .Select(x => x.Id).ToArray();
        }
        Guid[] farmIds = await db.Farms.AsNoTracking().Where(x => nodeIds.Contains(x.OrganisationNodeId))
            .Select(x => x.Id).ToArrayAsync(cancellationToken);
        var farms = new List<FarmProfitabilityResponse>();
        foreach (Guid farmId in farmIds)
            farms.Add(await BuildFarmProfitabilityAsync(farmId, from.ToUniversalTime(), to.ToUniversalTime(),
                normalizedCurrency, db, cancellationToken));
        decimal expenses = farms.Sum(x => x.Expenses); decimal revenue = farms.Sum(x => x.Revenue);
        EnterpriseConsolidationSummary? consolidation = await db.EnterpriseConsolidationRuns.AsNoTracking()
            .Where(x => x.OrganisationNodeId == organisationNodeId && x.PeriodStart == from.ToUniversalTime()
                && x.PeriodEnd == to.ToUniversalTime() && x.Currency == normalizedCurrency)
            .OrderByDescending(x => x.CompletedAt)
            .Select(x => new EnterpriseConsolidationSummary(x.Id, x.CompletedAt, x.EliminatedExpenses,
                x.EliminatedRevenue, x.ConsolidatedExpenses, x.ConsolidatedRevenue, x.ConsolidatedMargin,
                x.ExceptionCount, x.SourceSha256)).FirstOrDefaultAsync(cancellationToken);
        return UseCaseResults.Ok(new EnterpriseProfitabilityResponse(organisationNodeId, from.ToUniversalTime(),
            to.ToUniversalTime(), normalizedCurrency, expenses, revenue, revenue - expenses, farms, consolidation));
    }


    private static async Task<FarmProfitabilityResponse> BuildFarmProfitabilityAsync(Guid farmId,
        DateTimeOffset from, DateTimeOffset to, string currency, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        CostCentre[] centres = await db.CostCentres.AsNoTracking().Where(x => x.FarmId == farmId && x.Currency == currency)
            .OrderBy(x => x.Code).ToArrayAsync(cancellationToken);
        FinanceTransaction[] transactions = await db.FinanceTransactions.AsNoTracking().Where(x => x.FarmId == farmId
            && x.Currency == currency && x.OccurredAt >= from && x.OccurredAt < to).ToArrayAsync(cancellationToken);
        Guid[] transactionIds = transactions.Select(x => x.Id).ToArray();
        CostAllocation[] allocations = await db.CostAllocations.AsNoTracking()
            .Where(x => transactionIds.Contains(x.TransactionId)).ToArrayAsync(cancellationToken);
        var transactionById = transactions.ToDictionary(x => x.Id);

        var cropZones = await (from zone in db.CropZones.AsNoTracking()
            join season in db.GrowingSeasons.AsNoTracking() on zone.SeasonId equals season.Id
            where season.FarmId == farmId
            select new { zone.Id, zone.FieldId, zone.AreaHa }).ToArrayAsync(cancellationToken);
        var fieldAreas = await (from field in db.FarmFields.AsNoTracking()
            join boundary in db.FieldBoundaryVersions.AsNoTracking() on field.Id equals boundary.FieldId
            where field.FarmId == farmId && boundary.ValidTo == null
            select new { field.Id, boundary.AreaHa }).ToArrayAsync(cancellationToken);
        Guid[] cropZoneIds = cropZones.Select(x => x.Id).ToArray();
        var harvests = await db.HarvestLots.AsNoTracking().Where(x => cropZoneIds.Contains(x.CropZoneId)
                && x.Unit == "kg" && x.HarvestedAt >= from && x.HarvestedAt < to)
            .Select(x => new { x.CropZoneId, x.Quantity }).ToArrayAsync(cancellationToken);
        Guid[] livestockIds = centres.Where(x => x.Type == CostCentreType.LivestockSubject && x.TargetId != null)
            .Select(x => x.TargetId!.Value).Distinct().ToArray();
        LivestockSubject[] subjects = await db.LivestockSubjects.AsNoTracking().Where(x => livestockIds.Contains(x.Id))
            .ToArrayAsync(cancellationToken);
        var memberships = await db.LivestockGroupMemberships.AsNoTracking().Where(x => livestockIds.Contains(x.GroupId)
            && x.JoinedAt < to && (x.LeftAt == null || x.LeftAt >= to)).Select(x => new { x.GroupId }).ToArrayAsync(cancellationToken);

        var centreRows = new List<CostCentreProfitabilityResponse>();
        foreach (CostCentre centre in centres)
        {
            CostAllocation[] centreAllocations = allocations.Where(x => x.CostCentreId == centre.Id).ToArray();
            decimal expenses = centreAllocations.Where(x => transactionById[x.TransactionId].EntryType == FinanceEntryType.Expense)
                .Sum(x => x.Amount * (transactionById[x.TransactionId].ReversalOfId is null ? 1m : -1m));
            decimal revenue = centreAllocations.Where(x => transactionById[x.TransactionId].EntryType == FinanceEntryType.Revenue)
                .Sum(x => x.Amount * (transactionById[x.TransactionId].ReversalOfId is null ? 1m : -1m));
            decimal? areaHa = centre.Type switch
            {
                CostCentreType.Farm => fieldAreas.Sum(x => x.AreaHa),
                CostCentreType.Field => fieldAreas.Where(x => x.Id == centre.TargetId).Select(x => (decimal?)x.AreaHa).SingleOrDefault(),
                CostCentreType.CropZone => cropZones.Where(x => x.Id == centre.TargetId).Select(x => (decimal?)x.AreaHa).SingleOrDefault(),
                _ => null
            };
            Guid[] relevantZones = centre.Type switch
            {
                CostCentreType.Farm => cropZoneIds,
                CostCentreType.Field => cropZones.Where(x => x.FieldId == centre.TargetId).Select(x => x.Id).ToArray(),
                CostCentreType.CropZone when centre.TargetId is { } zoneId => [zoneId],
                _ => []
            };
            decimal yieldKg = harvests.Where(x => relevantZones.Contains(x.CropZoneId)).Sum(x => x.Quantity);
            decimal? headCount = null;
            if (centre.Type == CostCentreType.LivestockSubject && centre.TargetId is { } subjectId)
            {
                LivestockSubject? subject = subjects.SingleOrDefault(x => x.Id == subjectId);
                headCount = subject?.Kind == LivestockSubjectKind.Group
                    ? memberships.Count(x => x.GroupId == subjectId)
                    : subject is null ? null : 1m;
            }
            decimal margin = revenue - expenses;
            centreRows.Add(new CostCentreProfitabilityResponse(centre.Id, centre.Code, centre.Name, centre.Type,
                centre.TargetId, expenses, revenue, margin, areaHa, yieldKg, headCount,
                areaHa > 0 ? expenses / areaHa.Value : null,
                yieldKg > 0 ? expenses / (yieldKg / 1_000m) : null,
                areaHa > 0 ? margin / areaHa.Value : null,
                headCount > 0 ? expenses / headCount.Value : null,
                headCount > 0 ? margin / headCount.Value : null));
        }
        decimal totalExpenses = centreRows.Sum(x => x.Expenses); decimal totalRevenue = centreRows.Sum(x => x.Revenue);
        Guid[] reconcilableTransactionIds = transactions.Where(x => x.ReversalOfId is null).Select(x => x.Id).ToArray();
        int matchedTransactions = await db.AccountingReconciliations.AsNoTracking().CountAsync(x => x.TransactionId != null
            && reconcilableTransactionIds.Contains(x.TransactionId.Value), cancellationToken);
        decimal reconciliationCoverage = reconcilableTransactionIds.Length == 0 ? 100m
            : decimal.Round(100m * matchedTransactions / reconcilableTransactionIds.Length, 2, MidpointRounding.AwayFromZero);
        return new FarmProfitabilityResponse(farmId, from, to, currency, totalExpenses, totalRevenue,
            totalRevenue - totalExpenses, reconciliationCoverage, centreRows);
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

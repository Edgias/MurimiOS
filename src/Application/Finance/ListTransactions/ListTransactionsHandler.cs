using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ListTransactions;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;

public sealed class ListTransactionsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListTransactionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListTransactionsQuery message, CancellationToken cancellationToken) => ListTransactionsAsync(message.farmId, message.from, message.to, message.currency, message.costCentreId, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListTransactionsAsync(Guid farmId, DateTimeOffset? from,
        DateTimeOffset? to, string? currency, Guid? costCentreId, int? limit, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        int take = Math.Clamp(limit ?? 100, 1, 250);
        IQueryable<FinanceTransaction> query = db.FinanceTransactions.AsNoTracking().Where(x => x.FarmId == farmId);
        if (from is { } start) query = query.Where(x => x.OccurredAt >= start.ToUniversalTime());
        if (to is { } end) query = query.Where(x => x.OccurredAt < end.ToUniversalTime());
        if (!string.IsNullOrWhiteSpace(currency)) query = query.Where(x => x.Currency == currency.Trim().ToUpper());
        if (costCentreId is { } centreId)
        {
            Guid[] ids = await db.CostAllocations.AsNoTracking().Where(x => x.CostCentreId == centreId)
                .Select(x => x.TransactionId).ToArrayAsync(cancellationToken);
            query = query.Where(x => ids.Contains(x.Id));
        }
        FinanceTransaction[] transactions = await query.OrderByDescending(x => x.OccurredAt).ThenByDescending(x => x.Id)
            .Take(take).ToArrayAsync(cancellationToken);
        Guid[] transactionIds = transactions.Select(x => x.Id).ToArray();
        CostAllocation[] allocations = await db.CostAllocations.AsNoTracking()
            .Where(x => transactionIds.Contains(x.TransactionId)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(transactions.Select(x => ToTransaction(x, allocations.Where(a => a.TransactionId == x.Id))).ToArray());
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


    private static FinanceTransactionResponse ToTransaction(FinanceTransaction x,
        IEnumerable<CostAllocation> allocations) => new(x.Id, x.FarmId, x.EntryType, x.Category,
        x.Amount, x.SignedAmount, x.Currency, x.OccurredAt, x.SourceType, x.SourceId, x.Reference,
        x.Counterparty, x.Description, x.PostedAt, x.PostedBy, x.ReversalOfId,
        x.CounterpartyLegalCompanyId, x.IntercompanyReference,
        allocations.Select(a => new FinanceAllocationResponse(a.Id, a.CostCentreId, a.Amount,
            a.Percentage, a.Method, a.Notes)).ToArray());

}

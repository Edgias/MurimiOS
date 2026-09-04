using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.GetAccountingImport;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingMatchStatus = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingMatchStatus;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;

public sealed class GetAccountingImportHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetAccountingImportQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetAccountingImportQuery message, CancellationToken cancellationToken) => GetAccountingImportAsync(message.batchId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetAccountingImportAsync(Guid batchId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        AccountingImportBatch? batch = await db.AccountingImportBatches.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == batchId, cancellationToken);
        if (batch is null || !await scope.CanAccessFarmAsync(batch.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        AccountingStatementLine[] lines = await db.AccountingStatementLines.AsNoTracking()
            .Where(x => x.BatchId == batchId).OrderBy(x => x.OccurredAt).ToArrayAsync(cancellationToken);
        AccountingReconciliation[] reconciliations = await db.AccountingReconciliations.AsNoTracking()
            .Where(x => x.BatchId == batchId).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(ToAccountingImport(batch, lines, reconciliations));
    }


    private static AccountingImportResponse ToAccountingImport(AccountingImportBatch batch,
        IEnumerable<AccountingStatementLine> lines, IEnumerable<AccountingReconciliation> reconciliations)
    {
        AccountingReconciliation[] matchRows = reconciliations.ToArray();
        return new AccountingImportResponse(batch.Id, batch.FarmId, batch.Provider, batch.ExternalBatchId,
            batch.StatementStart, batch.StatementEnd, batch.Currency, batch.Status, batch.ImportedAt,
            batch.ImportedBy, batch.ReconciledAt, batch.ReconciledBy,
            lines.Select(line =>
            {
                AccountingReconciliation? match = matchRows.SingleOrDefault(x => x.StatementLineId == line.Id);
                return new AccountingStatementLineResponse(line.Id, line.ExternalLineId, line.OccurredAt,
                    line.EntryType, line.Amount, line.Currency, line.AccountCode, line.Reference,
                    line.Description, match?.Status, match?.TransactionId, match?.Difference,
                    match?.Tolerance, match?.Notes);
            }).ToArray());
    }

}

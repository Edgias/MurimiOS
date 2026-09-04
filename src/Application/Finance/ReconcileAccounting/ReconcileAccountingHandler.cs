using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ReconcileAccounting;
using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingMatchStatus = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingMatchStatus;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ReconcileAccountingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReconcileAccountingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReconcileAccountingCommand message, CancellationToken cancellationToken) => ReconcileAccountingAsync(message.batchId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReconcileAccountingAsync(Guid batchId, ReconcileAccountingRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (request.AmountTolerance < 0 || request.DateWindowDays is < 0 or > 31)
            return Bad("reconciliation_parameters_invalid", "Amount tolerance must be non-negative and the date window must be from 0 through 31 days.");
        AccountingImportBatch? batch = await db.AccountingImportBatches.SingleOrDefaultAsync(x => x.Id == batchId, cancellationToken);
        if (batch is null || !await scope.CanAccessFarmAsync(batch.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (batch.Status != AccountingImportStatus.Imported)
            return Conflict("accounting_import_reconciled", "This accounting import has already been reconciled.");
        AccountingStatementLine[] lines = await db.AccountingStatementLines.AsNoTracking().Where(x => x.BatchId == batchId)
            .OrderBy(x => x.OccurredAt).ThenBy(x => x.Id).ToArrayAsync(cancellationToken);
        DateTimeOffset candidateStart = new(batch.StatementStart.AddDays(-request.DateWindowDays).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        DateTimeOffset candidateEnd = new(batch.StatementEnd.AddDays(request.DateWindowDays + 1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        Guid[] alreadyMatchedIds = await db.AccountingReconciliations.AsNoTracking().Where(x => x.TransactionId != null)
            .Select(x => x.TransactionId!.Value).ToArrayAsync(cancellationToken);
        FinanceTransaction[] candidates = await db.FinanceTransactions.AsNoTracking().Where(x => x.FarmId == batch.FarmId
            && x.Currency == batch.Currency && x.ReversalOfId == null && x.OccurredAt >= candidateStart
            && x.OccurredAt < candidateEnd && !alreadyMatchedIds.Contains(x.Id)).ToArrayAsync(cancellationToken);
        var used = new HashSet<Guid>(); var results = new List<AccountingReconciliation>(); DateTimeOffset now = clock.GetUtcNow();
        foreach (AccountingStatementLine line in lines)
        {
            var ranked = candidates.Where(x => !used.Contains(x.Id) && x.EntryType == line.EntryType
                    && decimal.Abs(x.Amount - line.Amount) <= request.AmountTolerance
                    && Math.Abs((x.OccurredAt.Date - line.OccurredAt.Date).Days) <= request.DateWindowDays)
                .Select(x => new
                {
                    Transaction = x,
                    ReferenceRank = !string.IsNullOrWhiteSpace(line.Reference)
                        && x.Reference.Equals(line.Reference, StringComparison.OrdinalIgnoreCase) ? 0 : 1,
                    AmountDifference = decimal.Abs(x.Amount - line.Amount),
                    DateDifference = Math.Abs((x.OccurredAt.Date - line.OccurredAt.Date).Days)
                }).OrderBy(x => x.ReferenceRank).ThenBy(x => x.AmountDifference).ThenBy(x => x.DateDifference).ToArray();
            Guid? transactionId = null; decimal difference = line.Amount; string notes;
            if (ranked.Length == 0)
            {
                notes = "No internal transaction matched the type, currency, amount tolerance, and date window.";
            }
            else
            {
                var best = ranked[0];
                bool ambiguous = ranked.Skip(1).Any(x => x.ReferenceRank == best.ReferenceRank
                    && x.AmountDifference == best.AmountDifference && x.DateDifference == best.DateDifference);
                if (ambiguous) notes = "Multiple internal transactions have the same reconciliation score.";
                else
                {
                    transactionId = best.Transaction.Id; difference = best.Transaction.Amount - line.Amount;
                    used.Add(best.Transaction.Id); notes = "Matched automatically by reference, amount, and occurrence date.";
                }
            }
            results.Add(new AccountingReconciliation(Guid.NewGuid(), batch.Id, line.Id, transactionId,
                transactionId is null ? AccountingMatchStatus.Exception : AccountingMatchStatus.Matched,
                difference, request.AmountTolerance, now, actor.Subject, notes));
        }
        bool hasExceptions = results.Any(x => x.Status == AccountingMatchStatus.Exception);
        batch.CompleteReconciliation(hasExceptions, now, actor.Subject);
        db.AccountingReconciliations.AddRange(results);
        AddRecords(db, tenant, actor, "AccountingReconciled", "Finance.AccountingReconciled",
            nameof(AccountingImportBatch), batch.Id, batch.FarmId,
            new { batch.Id, batch.Status, Matched = results.Count(x => x.Status == AccountingMatchStatus.Matched),
                Exceptions = results.Count(x => x.Status == AccountingMatchStatus.Exception), request.AmountTolerance,
                request.DateWindowDays }, now, context);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToAccountingImport(batch, lines, results));
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


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid farmId, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(payload),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        string syncOperation = action.Contains("Created", StringComparison.Ordinal)
            || action.Contains("Posted", StringComparison.Ordinal)
            || action.Contains("Recorded", StringComparison.Ordinal)
            || action.Contains("Imported", StringComparison.Ordinal) ? "Created" : "Updated";
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, syncOperation,
            JsonSerializer.Serialize(payload), now));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType,
            JsonSerializer.Serialize(payload), now));
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

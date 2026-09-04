using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ImportAccounting;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingMatchStatus = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingMatchStatus;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ImportAccountingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ImportAccountingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ImportAccountingCommand message, CancellationToken cancellationToken) => ImportAccountingAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ImportAccountingAsync(Guid farmId, ImportAccountingRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        if (request.Lines is null || request.Lines.Count == 0) return Bad("accounting_lines_required", "At least one statement line is required.");
        if (request.Lines.Any(x => x.Id == Guid.Empty) || request.Lines.Select(x => x.Id).Distinct().Count() != request.Lines.Count
            || request.Lines.Select(x => x.ExternalLineId?.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Lines.Count)
            return Bad("accounting_lines_invalid", "Statement line identifiers and external line identifiers must be unique and non-empty.");
        Guid id = NewId(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var batch = new AccountingImportBatch(id, farmId, request.Provider, request.ExternalBatchId,
                request.StatementStart, request.StatementEnd, request.Currency, now, actor.Subject);
            AccountingStatementLine[] lines = request.Lines.Select(x => new AccountingStatementLine(x.Id, id,
                x.ExternalLineId, x.OccurredAt, x.EntryType, x.Amount, x.Currency, x.AccountCode,
                x.Reference, x.Description)).ToArray();
            if (lines.Any(x => x.Currency != batch.Currency
                || DateOnly.FromDateTime(x.OccurredAt.UtcDateTime) < batch.StatementStart
                || DateOnly.FromDateTime(x.OccurredAt.UtcDateTime) > batch.StatementEnd))
                return Bad("accounting_line_scope_invalid", "Every statement line must use the batch currency and fall within its period.");
            db.AccountingImportBatches.Add(batch); db.AccountingStatementLines.AddRange(lines);
            AddRecords(db, tenant, actor, "AccountingImportCreated", "Finance.AccountingImported",
                nameof(AccountingImportBatch), id, farmId,
                new { batch.Id, batch.Provider, batch.ExternalBatchId, LineCount = lines.Length, batch.Currency }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/finance/accounting-imports/{id}",
                ToAccountingImport(batch, lines, []));
        }
        catch (ArgumentException ex) { return Bad("accounting_import_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("accounting_import_exists", "The import batch or one of its external lines already exists."); }
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


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


    private static Guid NewId(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

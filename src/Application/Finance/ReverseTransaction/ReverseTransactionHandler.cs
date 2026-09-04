using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ReverseTransaction;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ReverseTransactionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReverseTransactionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReverseTransactionCommand message, CancellationToken cancellationToken) => ReverseTransactionAsync(message.transactionId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReverseTransactionAsync(Guid transactionId, ReverseFinanceTransactionRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        FinanceTransaction? original = await db.FinanceTransactions.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == transactionId, cancellationToken);
        if (original is null || !await scope.CanAccessFarmAsync(original.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (original.ReversalOfId is not null) return Conflict("reversal_invalid", "A reversal transaction cannot itself be reversed.");
        if (await db.FinanceTransactions.AnyAsync(x => x.ReversalOfId == original.Id, cancellationToken))
            return Conflict("transaction_already_reversed", "This transaction already has a reversal.");
        CostAllocation[] originalAllocations = await db.CostAllocations.AsNoTracking()
            .Where(x => x.TransactionId == original.Id).ToArrayAsync(cancellationToken);
        Guid id = NewId(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var reversal = new FinanceTransaction(id, original.FarmId, original.EntryType, original.Category,
                original.Amount, original.Currency, request.OccurredAt, FinanceSourceType.Reversal, original.Id,
                request.Reference, original.Counterparty, request.Reason, now, actor.Subject, original.Id,
                original.CounterpartyLegalCompanyId, original.IntercompanyReference);
            CostAllocation[] allocations = originalAllocations.Select(x => new CostAllocation(Guid.NewGuid(), id,
                x.CostCentreId, x.Amount, x.Percentage, x.Method, $"Reversal of allocation {x.Id}: {request.Reason}")).ToArray();
            db.FinanceTransactions.Add(reversal); db.CostAllocations.AddRange(allocations);
            AddRecords(db, tenant, actor, "FinanceTransactionReversed", "Finance.TransactionReversed",
                nameof(FinanceTransaction), id, original.FarmId,
                new { Reversal = ToTransaction(reversal, allocations), OriginalTransactionId = original.Id }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/finance/transactions/{id}", ToTransaction(reversal, allocations));
        }
        catch (ArgumentException ex) { return Bad("reversal_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("reversal_conflict", "A reversal or reference already exists."); }
    }


    private static FinanceTransactionResponse ToTransaction(FinanceTransaction x,
        IEnumerable<CostAllocation> allocations) => new(x.Id, x.FarmId, x.EntryType, x.Category,
        x.Amount, x.SignedAmount, x.Currency, x.OccurredAt, x.SourceType, x.SourceId, x.Reference,
        x.Counterparty, x.Description, x.PostedAt, x.PostedBy, x.ReversalOfId,
        x.CounterpartyLegalCompanyId, x.IntercompanyReference,
        allocations.Select(a => new FinanceAllocationResponse(a.Id, a.CostCentreId, a.Amount,
            a.Percentage, a.Method, a.Notes)).ToArray());


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

using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.CreateTransaction;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateTransactionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateTransactionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateTransactionCommand message, CancellationToken cancellationToken) => CreateTransactionAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateTransactionAsync(Guid farmId, CreateFinanceTransactionRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        if (request.SourceType is FinanceSourceType.Labour or FinanceSourceType.Machinery
            or FinanceSourceType.Reversal or FinanceSourceType.Planning)
            return Bad("finance_source_invalid", "Use the dedicated labour, machinery, planning, or reversal endpoint for this source type.");
        AllocationValidation? allocationValidation = await ValidateAllocationsAsync(farmId, request.Currency,
            request.Amount, request.Allocations, db, cancellationToken);
        if (allocationValidation.Error is not null) return Bad("allocation_invalid", allocationValidation.Error);
        string? intercompanyError = await ValidateIntercompanyAsync(farmId, request.CounterpartyLegalCompanyId,
            request.IntercompanyReference, db, cancellationToken);
        if (intercompanyError is not null) return Bad("intercompany_scope_invalid", intercompanyError);
        Guid id = NewId(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var transaction = new FinanceTransaction(id, farmId, request.EntryType, request.Category, request.Amount,
                request.Currency, request.OccurredAt, request.SourceType, request.SourceId, request.Reference,
                request.Counterparty, request.Description, now, actor.Subject, counterpartyLegalCompanyId: request.CounterpartyLegalCompanyId,
                intercompanyReference: request.IntercompanyReference);
            CostAllocation[] allocations = BuildAllocations(id, request.Amount, request.Allocations);
            db.FinanceTransactions.Add(transaction); db.CostAllocations.AddRange(allocations);
            AddRecords(db, tenant, actor, "FinanceTransactionPosted", "Finance.TransactionPosted",
                nameof(FinanceTransaction), id, farmId, ToTransaction(transaction, allocations), now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/finance/transactions/{id}", ToTransaction(transaction, allocations));
        }
        catch (ArgumentException ex) { return Bad("finance_transaction_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("finance_transaction_conflict", ex.InnerException?.Message ?? "The transaction reference or source already exists."); }
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


    private static async Task<AllocationValidation> ValidateAllocationsAsync(Guid farmId, string currency,
        decimal amount, IReadOnlyList<FinanceAllocationRequest>? requests, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        if (amount <= 0) return new("Transaction amount must be positive.");
        if (requests is null || requests.Count == 0) return new("At least one cost allocation is required.");
        if (requests.Any(x => x.CostCentreId == Guid.Empty) || requests.Select(x => x.CostCentreId).Distinct().Count() != requests.Count)
            return new("Cost centres must be unique and non-empty within a transaction.");
        if (requests.Any(x => x.Amount <= 0 || !Enum.IsDefined(x.Method))) return new("Allocation amounts and methods must be valid.");
        if (decimal.Round(requests.Sum(x => x.Amount), 2, MidpointRounding.AwayFromZero)
            != decimal.Round(amount, 2, MidpointRounding.AwayFromZero)) return new("Allocation amounts must equal the transaction amount.");
        Guid[] centreIds = requests.Select(x => x.CostCentreId).ToArray();
        CostCentre[] centres = await db.CostCentres.AsNoTracking().Where(x => centreIds.Contains(x.Id)
            && x.FarmId == farmId && x.IsActive).ToArrayAsync(cancellationToken);
        if (centres.Length != centreIds.Length) return new("Every cost centre must be active and belong to this farm.");
        if (centres.Any(x => !x.Currency.Equals(currency?.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new("Every cost centre must use the transaction currency.");
        foreach (FinanceAllocationRequest request in requests.Where(x => x.Percentage is not null))
        {
            decimal calculated = 100m * request.Amount / amount;
            if (decimal.Abs(calculated - request.Percentage!.Value) > 0.0001m)
                return new("A supplied allocation percentage does not agree with its amount.");
        }
        return new(null);
    }


    private static async Task<string?> ValidateIntercompanyAsync(Guid farmId, Guid? counterpartyCompanyId,
        string? intercompanyReference, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        if ((counterpartyCompanyId is null) != string.IsNullOrWhiteSpace(intercompanyReference))
            return "A counterparty legal company and intercompany reference must be supplied together.";
        if (counterpartyCompanyId is null) return null;
        LegalCompany? counterparty = await db.LegalCompanies.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == counterpartyCompanyId && x.IsActive, cancellationToken);
        if (counterparty is null) return "The counterparty legal company is unavailable.";
        string? farmPath = await (from farm in db.Farms.AsNoTracking()
            join node in db.OrganisationNodes.AsNoTracking() on farm.OrganisationNodeId equals node.Id
            where farm.Id == farmId select node.Path).SingleOrDefaultAsync(cancellationToken);
        if (farmPath is null) return "The farm organisation path is unavailable.";
        var companies = await (from company in db.LegalCompanies.AsNoTracking()
            join node in db.OrganisationNodes.AsNoTracking() on company.OrganisationNodeId equals node.Id
            where company.IsActive select new { company.Id, node.Path }).ToArrayAsync(cancellationToken);
        Guid? owner = companies.Where(x => farmPath == x.Path || farmPath.StartsWith(x.Path + ".", StringComparison.Ordinal))
            .OrderByDescending(x => x.Path.Length).Select(x => (Guid?)x.Id).FirstOrDefault();
        if (owner is null) return "The farm must belong to a registered legal company before posting intercompany transactions.";
        if (owner == counterpartyCompanyId) return "An intercompany counterparty must be a different legal company.";
        return null;
    }


    private static CostAllocation[] BuildAllocations(Guid transactionId, decimal total,
        IReadOnlyList<FinanceAllocationRequest> requests) => requests.Select(x => new CostAllocation(
            NewId(x.Id), transactionId, x.CostCentreId, x.Amount,
            x.Percentage ?? 100m * x.Amount / total, x.Method, x.Notes)).ToArray();


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

    private sealed record AllocationValidation(string? Error);

}

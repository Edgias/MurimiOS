using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.Reconcile;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractReconciliation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliation;
using ContractReconciliationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;

public sealed class ReconcileHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<ReconcileCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReconcileCommand message, CancellationToken cancellationToken) => ReconcileAsync(message.contractId, message.request, db, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> ReconcileAsync(Guid contractId, ReconcileContractRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context,
        TimeProvider clock, CancellationToken ct)
    {
        SupplyContract? contract = await db.SupplyContracts.SingleOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return UseCaseResults.NotFound();
        if (contract.Status is not (SupplyContractStatus.Active or SupplyContractStatus.Completed))
            return Conflict("contract_not_reconcilable", "Only active or completed contracts can be reconciled.");
        decimal fulfilled = await db.ContractAllocations.Where(x => x.ContractId == contractId
            && x.Status != ContractAllocationStatus.Cancelled).SumAsync(x => x.FulfilledQuantity, ct);
        if (request.AcceptedQuantity + request.RejectedQuantity != fulfilled)
            return Bad("reconciliation_quantity_mismatch", "Accepted plus rejected quantity must equal recorded fulfilment.");
        ContractPriceRule[] rules = await db.ContractPriceRules.AsNoTracking().Where(x => x.ContractId == contractId).ToArrayAsync(ct);
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            ContractPriceQuote quote = ContractPricing.Resolve(contract, rules, fulfilled, request.QualityGrade,
                request.EffectiveOn ?? DateOnly.FromDateTime(now.UtcDateTime));
            decimal gross = decimal.Round(request.AcceptedQuantity * quote.UnitPrice, 2, MidpointRounding.AwayFromZero);
            decimal payable = decimal.Round(gross + request.QualityAdjustmentAmount + request.OtherAdjustmentAmount,
                2, MidpointRounding.AwayFromZero);
            if (payable < 0) return Bad("reconciliation_payable_negative", "Adjustments cannot produce a negative payable amount.");
            int revision = (await db.ContractReconciliations.Where(x => x.ContractId == contractId)
                .MaxAsync(x => (int?)x.Revision, ct) ?? 0) + 1;
            ContractReconciliationStatus status = request.RejectedQuantity > 0 || fulfilled != contract.ContractedQuantity
                || request.QualityAdjustmentAmount != 0 || request.OtherAdjustmentAmount != 0
                ? ContractReconciliationStatus.Exception : ContractReconciliationStatus.Matched;
            var reconciliation = new ContractReconciliation(Id(request.Id), contractId, revision, request.Reference,
                quote.PriceRuleId, fulfilled, request.AcceptedQuantity, request.RejectedQuantity, contract.QuantityUnit,
                quote.UnitPrice, gross, request.QualityAdjustmentAmount, request.OtherAdjustmentAmount, payable,
                quote.Currency, status, request.Notes, now, actor.Subject);
            db.ContractReconciliations.Add(reconciliation);
            AddRecords(db, tenant, actor.Subject, actor.MembershipId, actor.ScopeOrganisationNodeId, "ContractReconciled",
                reconciliation.Id, reconciliation, context, now);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/contracts/{contractId}/reconciliations/{reconciliation.Id}", reconciliation);
        }
        catch (DbUpdateException) { return Conflict("reconciliation_conflict", "The reconciliation reference or revision already exists."); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Bad("reconciliation_invalid", e.Message); }
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, string actor,
        Guid? membershipId, Guid? scopeId, string action, Guid entityId, object payload,
        UseCaseContext context, DateTimeOffset now)
    {
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor, membershipId,
            action, "SupplierWorkflow", entityId, null, scopeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"Supplier.{action}", json, now));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

}

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.RecordFinancialEntry;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RecordFinancialEntryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<RecordFinancialEntryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RecordFinancialEntryCommand message, CancellationToken cancellationToken) => RecordFinancialEntryAsync(message.planId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RecordFinancialEntryAsync(Guid planId, RecordCropPlanFinancialEntryRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        CropPlan? plan = await db.CropPlans.SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (plan.Status is not (CropPlanStatus.Approved or CropPlanStatus.Superseded))
            return Conflict("plan_not_active", "Actual financial entries can be recorded only against an approved plan or its retained predecessor.");
        if (!plan.Currency.Equals(request.Currency?.Trim(), StringComparison.OrdinalIgnoreCase))
            return Bad("currency_mismatch", $"Financial entries for this plan must use {plan.Currency}.");
        CropPlanAllocation? allocation = request.AllocationId is { } allocationId
            ? await db.CropPlanAllocations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == allocationId && x.PlanId == planId, cancellationToken)
            : null;
        if (request.AllocationId is not null && allocation is null)
            return Bad("allocation_invalid", "The financial allocation does not belong to this plan.");
        CostCentre? costCentre;
        if (request.CostCentreId is { } requestedCentreId)
        {
            costCentre = await db.CostCentres.AsNoTracking().SingleOrDefaultAsync(x => x.Id == requestedCentreId
                && x.FarmId == plan.FarmId && x.Currency == plan.Currency && x.IsActive, cancellationToken);
        }
        else if (allocation is not null)
        {
            CostCentre[] candidates = await db.CostCentres.AsNoTracking().Where(x => x.FarmId == plan.FarmId
                && x.Currency == plan.Currency && x.IsActive
                && ((x.Type == CostCentreType.CropZone && x.TargetId == allocation.CropZoneId)
                    || (x.Type == CostCentreType.Field && x.TargetId == allocation.FieldId))).ToArrayAsync(cancellationToken);
            costCentre = candidates.OrderBy(x => x.Type == CostCentreType.CropZone ? 0 : 1).FirstOrDefault();
        }
        else
        {
            costCentre = await db.CostCentres.AsNoTracking().Where(x => x.FarmId == plan.FarmId
                && x.Currency == plan.Currency && x.IsActive
                && ((x.Type == CostCentreType.Farm && x.TargetId == plan.FarmId) || x.Type == CostCentreType.Overhead))
                .OrderBy(x => x.Type == CostCentreType.Farm ? 0 : 1).FirstOrDefaultAsync(cancellationToken);
        }
        if (costCentre is null)
            return Conflict("finance_cost_centre_required", "Configure an active matching finance cost centre or supply costCentreId before recording plan actuals.");
        Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var entry = new CropPlanFinancialEntry(id, planId, request.AllocationId, request.EntryType,
                request.Category, request.Amount, request.Currency, request.OccurredAt, request.Reference,
                request.Notes, actor.Subject);
            Guid financeTransactionId = Guid.NewGuid();
            var financeTransaction = new FinanceTransaction(financeTransactionId, plan.FarmId,
                request.EntryType == FinancialEntryType.Cost ? FinanceEntryType.Expense : FinanceEntryType.Revenue,
                request.Category, request.Amount, request.Currency, request.OccurredAt, FinanceSourceType.Planning,
                entry.Id, request.Reference, null, request.Notes, now, actor.Subject);
            var costAllocation = new CostAllocation(Guid.NewGuid(), financeTransactionId, costCentre.Id,
                request.Amount, 100m, CostAllocationMethod.Direct, $"Crop plan {plan.Id}, allocation {request.AllocationId}");
            db.CropPlanFinancialEntries.Add(entry);
            db.FinanceTransactions.Add(financeTransaction);
            db.CostAllocations.Add(costAllocation);
            AddChange(db, tenant.TenantId, plan.FarmId, "CropPlanFinancialEntry", id, "Created", ToFinancial(entry), now);
            AddChange(db, tenant.TenantId, plan.FarmId, "FinanceTransaction", financeTransactionId, "Created",
                new { financeTransaction.Id, financeTransaction.EntryType, financeTransaction.Amount,
                    financeTransaction.Currency, financeTransaction.SourceType, financeTransaction.SourceId,
                    CostCentreId = costCentre.Id }, now);
            AddAudit(db, tenant, actor, "FinancialRecorded", plan, plan.FarmId,
                new { entry.Id, entry.EntryType, entry.Category, entry.Amount, entry.Currency, entry.Reference,
                    FinanceTransactionId = financeTransaction.Id, CostCentreId = costCentre.Id }, now, context, entry.Id);
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "CropPlan.FinancialRecorded",
                JsonSerializer.Serialize(new { entry.Id, entry.PlanId, entry.AllocationId, entry.EntryType, entry.Amount,
                    entry.Currency, entry.OccurredAt, FinanceTransactionId = financeTransaction.Id,
                    CostCentreId = costCentre.Id }), now));
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Finance.TransactionPosted",
                JsonSerializer.Serialize(new { financeTransaction.Id, financeTransaction.FarmId,
                    financeTransaction.EntryType, financeTransaction.Amount, financeTransaction.Currency,
                    financeTransaction.SourceType, financeTransaction.SourceId, CostCentreId = costCentre.Id }), now));
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/crop-planning/plans/{planId}/financial-entries/{id}", ToFinancial(entry));
        }
        catch (ArgumentException ex) { return Bad("financial_entry_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("financial_reference_exists", "The financial reference already exists in this tenant."); }
    }

    private static CropPlanFinancialEntryResponse ToFinancial(CropPlanFinancialEntry x) => new(x.Id, x.PlanId,
        x.AllocationId, x.EntryType, x.Category, x.Amount, x.Currency, x.OccurredAt, x.Reference, x.Notes, x.RecordedBy);

    private static void AddChange(IPlatformDbContext db, Guid tenantId, Guid farmId, string entityType,
        Guid entityId, string operation, object payload, DateTimeOffset now) =>
        db.SyncChanges.Add(new SyncChange(tenantId, farmId, entityType, entityId, operation, JsonSerializer.Serialize(payload), now));

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, CropPlan? plan, Guid farmId, object changes, DateTimeOffset now, UseCaseContext context, Guid? entityId = null) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, plan is null ? nameof(CropProductApplicationRule) : nameof(CropPlan), entityId ?? plan!.Id,
            farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.CreatePriceRule;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;

public sealed class CreatePriceRuleHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<CreatePriceRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreatePriceRuleCommand message, CancellationToken cancellationToken) => CreatePriceRuleAsync(message.contractId, message.request, db, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> CreatePriceRuleAsync(Guid contractId, CreateContractPriceRuleRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context,
        TimeProvider clock, CancellationToken ct)
    {
        SupplyContract? contract = await db.SupplyContracts.SingleOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return UseCaseResults.NotFound();
        if (contract.Status != SupplyContractStatus.Draft) return Conflict("contract_pricing_frozen", "Price rules are immutable after contract activation.");
        if (request.EffectiveFrom < contract.ValidFrom || request.EffectiveTo > contract.ValidTo)
            return Bad("price_period_invalid", "The price rule must fall within the contract period.");
        if (contract.Currency is not null && !contract.Currency.Equals(request.Currency, StringComparison.OrdinalIgnoreCase))
            return Bad("price_currency_mismatch", "All contract prices must use the contract currency.");
        if (await db.ContractPriceRules.AnyAsync(x => x.ContractId == contractId && x.Priority == request.Priority, ct))
            return Conflict("price_priority_exists", "Price-rule priority must be unique within a contract.");
        try
        {
            var rule = new ContractPriceRule(Id(request.Id), contractId, request.Priority, request.Method,
                request.MinimumQuantity, request.MaximumQuantity, request.QualityGrade, request.UnitPrice,
                request.Currency, request.EffectiveFrom, request.EffectiveTo);
            db.ContractPriceRules.Add(rule);
            DateTimeOffset now = clock.GetUtcNow();
            AddRecords(db, tenant, actor.Subject, actor.MembershipId, actor.ScopeOrganisationNodeId, "PriceRuleCreated",
                rule.Id, rule, context, now);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/contracts/{contractId}/price-rules/{rule.Id}", rule);
        }
        catch (ArgumentException e) { return Bad("price_rule_invalid", e.Message); }
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

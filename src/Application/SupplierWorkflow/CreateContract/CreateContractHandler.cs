using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateContract;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class CreateContractHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<CreateContractCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateContractCommand message, CancellationToken cancellationToken) => CreateContractAsync(message.request, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> CreateContractAsync(CreateSupplyContractRequest request, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        UseCaseResult? targetError = await ValidateTargetAsync(request.RelationshipId, request.HostedSupplierId, db, tenant, ct);
        if (targetError is not null) return targetError;
        try
        {
            var contract = new SupplyContract(Id(request.Id), request.ContractNumber, request.RelationshipId,
                request.HostedSupplierId, request.Commodity, request.ValidFrom, request.ValidTo, request.ContractedQuantity,
                request.QuantityUnit, request.UnitPrice, request.Currency, Json(request.Terms));
            db.SupplyContracts.Add(contract); AddRecords(db, tenant, actor, "Created", contract.Id, contract, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/supplier-portal/contracts/{contract.Id}", contract);
        }
        catch (ArgumentException e) { return Bad("contract_invalid", e.Message); }
    }


    private static async Task<UseCaseResult?> ValidateTargetAsync(Guid? relationshipId, Guid? hostedSupplierId,
        IPlatformDbContext db, ITenantContext tenant, CancellationToken ct)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue) return Bad("supplier_target_invalid", "Choose either an active tenant relationship or hosted supplier.");
        if (relationshipId.HasValue && !await db.SupplyRelationships.AnyAsync(x => x.Id == relationshipId && x.Status == SupplyRelationshipStatus.Active
            && (x.ProducerTenantId == tenant.TenantId || x.BuyerTenantId == tenant.TenantId), ct)) return Bad("relationship_invalid", "The active supply relationship is not available.");
        if (hostedSupplierId.HasValue && !await db.HostedSupplierAccounts.AnyAsync(x => x.Id == hostedSupplierId && x.Status != HostedSupplierStatus.Suspended, ct)) return Bad("hosted_supplier_invalid", "The hosted supplier is not active.");
        return null;
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, Guid entityId, object payload, UseCaseContext context)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, "SupplierWorkflow", entityId, null, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"Supplier{action}", json, now));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static string? Json(JsonElement? value) => value.HasValue ? value.Value.GetRawText() : null;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

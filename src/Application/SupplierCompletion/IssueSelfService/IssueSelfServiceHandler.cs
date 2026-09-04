using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.IssueSelfService;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using SupplierSelfServiceTokenStatus = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceTokenStatus;

public sealed class IssueSelfServiceHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<IssueSelfServiceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IssueSelfServiceCommand message, CancellationToken cancellationToken) => IssueSelfServiceAsync(message.supplierId, message.request, db, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> IssueSelfServiceAsync(Guid supplierId, IssueSupplierSelfServiceRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context,
        TimeProvider clock, CancellationToken ct)
    {
        HostedSupplierAccount? supplier = await db.HostedSupplierAccounts.SingleOrDefaultAsync(x => x.Id == supplierId, ct);
        if (supplier is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        if (request.ExpiresAt <= now || request.ExpiresAt > now.AddDays(7))
            return Bad("self_service_expiry_invalid", "Supplier self-service actions must expire within seven days.");
        if (request.Action == SupplierSelfServiceAction.Claim && supplier.Status != HostedSupplierStatus.Active)
            return Conflict("supplier_not_claimable", "Only an active hosted supplier can receive a claim action.");
        if (request.Action == SupplierSelfServiceAction.Detach
            && (supplier.Status != HostedSupplierStatus.Claimed || supplier.ClaimedTenantId != request.ClaimedTenantId))
            return Conflict("supplier_not_detachable", "The detach target must be the tenant currently linked to this supplier.");
        SupplierSelfServiceToken[] previous = await db.SupplierSelfServiceTokens.Where(x => x.HostedSupplierId == supplierId
            && x.Action == request.Action && x.Status == SupplierSelfServiceTokenStatus.Pending).ToArrayAsync(ct);
        foreach (SupplierSelfServiceToken item in previous) item.Revoke(now);
        string raw = $"mro_supplier_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}";
        var token = new SupplierSelfServiceToken(Guid.NewGuid(), supplierId, request.Action, request.ClaimedTenantId,
            Hash(raw), request.ExpiresAt, now, actor.Subject);
        db.SupplierSelfServiceTokens.Add(token);
        AddRecords(db, tenant, actor.Subject, actor.MembershipId, actor.ScopeOrganisationNodeId, "SelfServiceIssued",
            token.Id, new { token.Id, token.HostedSupplierId, token.Action, token.ClaimedTenantId, token.ExpiresAt }, context, now);
        await db.SaveChangesAsync(ct);
        return UseCaseResults.Created($"/api/v1/supplier-portal/self-service-actions/{token.Id}", new
        {
            token = new { token.Id, token.HostedSupplierId, token.Action, token.ClaimedTenantId, token.Status, token.ExpiresAt },
            secret = raw,
            publicUrl = $"/api/public/v1/supplier-accounts/actions/{raw}"
        });
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


    private static string Hash(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value.Trim())));

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

}

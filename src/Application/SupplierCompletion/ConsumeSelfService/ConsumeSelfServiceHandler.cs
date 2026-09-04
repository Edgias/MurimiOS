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
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.ConsumeSelfService;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierLinkEvent = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierLinkEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using SupplierSelfServiceTokenStatus = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceTokenStatus;

public sealed class ConsumeSelfServiceHandler(IPlatformDbContext db, IMutableTenantContext tenant, UseCaseContext context, TimeProvider clock) : IRequestHandler<ConsumeSelfServiceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ConsumeSelfServiceCommand message, CancellationToken cancellationToken) => ConsumeSelfServiceAsync(message.token, db, tenant, context, clock, cancellationToken);

    private static async Task<UseCaseResult> ConsumeSelfServiceAsync(string token, IPlatformDbContext db,
        IMutableTenantContext tenant, UseCaseContext context, TimeProvider clock, CancellationToken ct)
    {
        SupplierSelfServiceIdentity? identity = await ResolveSelfServiceAsync(token, db, ct);
        if (identity is null) return UseCaseResults.NotFound();
        tenant.Resolve(identity.TenantId);
        SupplierSelfServiceToken action = await db.SupplierSelfServiceTokens.SingleAsync(x => x.Id == identity.TokenId, ct);
        HostedSupplierAccount supplier = await db.HostedSupplierAccounts.SingleAsync(x => x.Id == identity.SupplierId, ct);
        DateTimeOffset now = clock.GetUtcNow();
        if (action.Status != SupplierSelfServiceTokenStatus.Pending)
            return UseCaseResults.Problem(statusCode: 410, title: "Supplier action unavailable", detail: "This action has already been used or revoked.");
        if (action.ExpiresAt <= now)
        {
            action.Expire(now); await db.SaveChangesAsync(ct);
            return UseCaseResults.Problem(statusCode: 410, title: "Supplier action expired", detail: "Request a new supplier action link.");
        }
        try
        {
            if (action.Action == SupplierSelfServiceAction.Claim) supplier.Claim(action.ClaimedTenantId, now);
            else supplier.Detach(action.ClaimedTenantId, now);
            action.Consume(now, RemoteHash(context));
            var link = new HostedSupplierLinkEvent(Guid.NewGuid(), supplier.Id, action.Id, action.Action,
                action.ClaimedTenantId, now);
            db.HostedSupplierLinkEvents.Add(link);
            string actor = $"supplier-self-service:{action.ClaimedTenantId:D}";
            AddRecords(db, tenant, actor, null, null, action.Action == SupplierSelfServiceAction.Claim ? "Claimed" : "Detached",
                link.Id, new { supplierId = supplier.Id, action.Action, action.ClaimedTenantId, tokenId = action.Id }, context, now);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Ok(new { supplier.Id, supplier.Status, action = action.Action, action.ClaimedTenantId, completedAt = now });
        }
        catch (DbUpdateConcurrencyException) { return Conflict("self_service_already_consumed", "The supplier action was consumed concurrently."); }
        catch (InvalidOperationException e) { return Conflict("supplier_link_transition_invalid", e.Message); }
    }


    private static async Task<SupplierSelfServiceIdentity?> ResolveSelfServiceAsync(string raw, IPlatformDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw.Length > 200) return null;
        SupplierSelfServiceIdentity? result = null;
        await db.Database.OpenConnectionAsync(ct);
        try
        {
            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "select tenant_id,token_id,supplier_id from supply.resolve_supplier_self_service_token(@hash)";
            var parameter = command.CreateParameter(); parameter.ParameterName = "hash"; parameter.Value = Hash(raw); command.Parameters.Add(parameter);
            await using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct)) result = new(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2));
        }
        finally { await db.Database.CloseConnectionAsync(); }
        return result;
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

    private static string? RemoteHash(UseCaseContext context) => context.User.RemoteAddress is { } address ? Hash(address.ToString()) : null;

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private sealed record SupplierSelfServiceIdentity(Guid TenantId, Guid TokenId, Guid SupplierId);

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

}

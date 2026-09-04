using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.RevokeSelfService;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;

public sealed class RevokeSelfServiceHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<RevokeSelfServiceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeSelfServiceCommand message, CancellationToken cancellationToken) => RevokeSelfServiceAsync(message.tokenId, db, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> RevokeSelfServiceAsync(Guid tokenId, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock, CancellationToken ct)
    {
        SupplierSelfServiceToken? token = await db.SupplierSelfServiceTokens.SingleOrDefaultAsync(x => x.Id == tokenId, ct);
        if (token is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); token.Revoke(now);
            AddRecords(db, tenant, actor.Subject, actor.MembershipId, actor.ScopeOrganisationNodeId,
                "SelfServiceRevoked", token.Id, new { token.Id, token.HostedSupplierId, token.Action }, context, now);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(token);
        }
        catch (InvalidOperationException e) { return Conflict("self_service_transition_invalid", e.Message); }
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

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

}

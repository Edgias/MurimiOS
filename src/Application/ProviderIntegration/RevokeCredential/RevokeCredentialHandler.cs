using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.RevokeCredential;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RevokeCredentialHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RevokeCredentialCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeCredentialCommand message, CancellationToken cancellationToken) => RevokeCredentialAsync(message.connectionId, message.credentialId, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RevokeCredentialAsync(Guid connectionId, Guid credentialId,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        IntegrationCredential? credential = await db.IntegrationCredentials.SingleOrDefaultAsync(x => x.Id == credentialId && x.ConnectionId == connectionId, cancellationToken);
        if (credential is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try { credential.Revoke(now, actor.Subject); }
        catch (InvalidOperationException ex) { return Conflict("integration_credential_not_revocable", ex.Message); }
        IntegrationConnection connection = await db.IntegrationConnections.SingleAsync(x => x.Id == connectionId, cancellationToken);
        if (connection.Status == IntegrationConnectionStatus.Active) connection.Pause();
        AddEvent(db, tenant, actor, nameof(IntegrationCredential), credential.Id, null, "CredentialRevoked",
            "Integration.CredentialRevoked", new { credential.Id, credential.ConnectionId, credential.CredentialVersion, credential.Fingerprint }, now, context);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToCredential(credential));
    }

    private static IntegrationCredentialResponse ToCredential(IntegrationCredential x) => new(x.Id, x.ConnectionId,
        x.CredentialVersion, x.SecretProvider, x.Fingerprint, x.ValidFrom, x.ExpiresAt, x.CreatedAt, x.CreatedBy,
        x.PreviousCredentialId, x.Status, x.SupersededAt, x.SupersededByCredentialId, x.RevokedAt, x.RevokedBy);

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, action, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

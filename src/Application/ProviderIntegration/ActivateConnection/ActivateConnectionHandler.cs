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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ActivateConnection;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ActivateConnectionHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ActivateConnectionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ActivateConnectionCommand message, CancellationToken cancellationToken) => ActivateConnectionAsync(message.connectionId, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ActivateConnectionAsync(Guid connectionId, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IntegrationConnection? connection = await db.IntegrationConnections.SingleOrDefaultAsync(x => x.Id == connectionId, cancellationToken);
        if (connection is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        bool hasCredential = await db.IntegrationCredentials.AnyAsync(x => x.ConnectionId == connectionId
            && x.Status == IntegrationCredentialStatus.Active && x.ValidFrom <= now && (x.ExpiresAt == null || x.ExpiresAt > now), cancellationToken);
        if (!hasCredential) return Conflict("active_credential_required", "An active, currently valid credential reference is required.");
        try { connection.Activate(now); }
        catch (InvalidOperationException ex) { return Conflict("integration_connection_transition", ex.Message); }
        AddEvent(db, tenant, actor, nameof(IntegrationConnection), connection.Id, null, "ConnectionActivated",
            "Integration.ConnectionActivated", new { connection.Id, connection.Status }, now, context);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToConnection(connection, null, 0));
    }


    private static IntegrationConnectionResponse ToConnection(IntegrationConnection x, ActiveCredentialProjection? credential, int feedCount) =>
        new(x.Id, x.ProviderCode, x.Name, x.Category, x.Status, x.Health, x.CreatedAt, x.CreatedBy, x.ActivatedAt,
            x.LastSucceededAt, x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.RateLimitRemaining,
            x.RateLimitResetAt, x.DisabledAt, credential?.CredentialVersion, credential?.Fingerprint, credential?.ExpiresAt, feedCount, x.Version);

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

    private sealed record ActiveCredentialProjection(Guid ConnectionId, int CredentialVersion, string Fingerprint, DateTimeOffset? ExpiresAt);

}

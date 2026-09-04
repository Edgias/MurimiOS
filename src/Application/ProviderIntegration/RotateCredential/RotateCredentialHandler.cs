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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.RotateCredential;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RotateCredentialHandler(IPlatformDbContext db, IIntegrationCredentialProtector protector, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RotateCredentialCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RotateCredentialCommand message, CancellationToken cancellationToken) => RotateCredentialAsync(message.connectionId, message.request, db, protector, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RotateCredentialAsync(Guid connectionId, RotateIntegrationCredentialRequest request,
        IPlatformDbContext db, IIntegrationCredentialProtector protector, ITenantContext tenant,
        IRequestActorContext actor, TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IntegrationConnection? connection = await db.IntegrationConnections.SingleOrDefaultAsync(x => x.Id == connectionId, cancellationToken);
        if (connection is null) return UseCaseResults.NotFound();
        if (connection.Status == IntegrationConnectionStatus.Disabled) return Conflict("integration_connection_disabled", "Credentials cannot be rotated on a disabled connection.");
        DateTimeOffset now = clock.GetUtcNow();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
            {
                string lockKey = $"integration-credential:{connectionId:N}";
                await db.Database.ExecuteSqlInterpolatedAsync(
                    $"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
            }
            IntegrationCredential? current = await db.IntegrationCredentials.SingleOrDefaultAsync(x => x.ConnectionId == connectionId && x.Status == IntegrationCredentialStatus.Active, cancellationToken);
            int version = (await db.IntegrationCredentials.Where(x => x.ConnectionId == connectionId).MaxAsync(x => (int?)x.CredentialVersion, cancellationToken) ?? 0) + 1;
            Guid id = NonEmptyOrNew(request.Id); var protectedValue = protector.Protect(request.SecretReference);
            var credential = new IntegrationCredential(id, connectionId, version, request.SecretProvider,
                protectedValue.ProtectedReference, protectedValue.Fingerprint, request.ValidFrom ?? now,
                request.ExpiresAt, now, actor.Subject, current?.Id);
            if (current is not null) current.Supersede(id, now);
            db.IntegrationCredentials.Add(credential);
            AddEvent(db, tenant, actor, nameof(IntegrationCredential), id, null, "CredentialRotated",
                "Integration.CredentialRotated", new { credential.Id, credential.ConnectionId, credential.CredentialVersion,
                    credential.SecretProvider, credential.Fingerprint, credential.ValidFrom, credential.ExpiresAt, credential.PreviousCredentialId }, now, context);
            await db.SaveChangesAsync(cancellationToken); await transaction.CommitAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/integrations/connections/{connectionId}/credentials/{id}", ToCredential(credential));
        }
        catch (ArgumentException ex) { await transaction.RollbackAsync(cancellationToken); return Bad("integration_credential_invalid", ex.Message); }
        catch (InvalidOperationException ex) { await transaction.RollbackAsync(cancellationToken); return Conflict("integration_credential_conflict", ex.Message); }
        catch (DbUpdateException ex) { await transaction.RollbackAsync(cancellationToken); return Conflict("integration_credential_conflict", DbMessage(ex)); }
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

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.CreateConnection;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateConnectionHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateConnectionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateConnectionCommand message, CancellationToken cancellationToken) => CreateConnectionAsync(message.request, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateConnectionAsync(CreateIntegrationConnectionRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        try
        {
            string configuration = SafeConfiguration(request.Configuration);
            Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
            var connection = new IntegrationConnection(id, request.ProviderCode, request.Name, request.Category,
                configuration, now, actor.Subject);
            db.IntegrationConnections.Add(connection);
            AddEvent(db, tenant, actor, nameof(IntegrationConnection), id, null, "ConnectionCreated",
                "Integration.ConnectionCreated", new { connection.Id, connection.ProviderCode, connection.Name, connection.Category }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/integrations/connections/{id}", ToConnection(connection, null, 0));
        }
        catch (ArgumentException ex) { return Bad("integration_connection_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("integration_connection_conflict", DbMessage(ex)); }
    }


    private static IntegrationConnectionResponse ToConnection(IntegrationConnection x, ActiveCredentialProjection? credential, int feedCount) =>
        new(x.Id, x.ProviderCode, x.Name, x.Category, x.Status, x.Health, x.CreatedAt, x.CreatedBy, x.ActivatedAt,
            x.LastSucceededAt, x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.RateLimitRemaining,
            x.RateLimitResetAt, x.DisabledAt, credential?.CredentialVersion, credential?.Fingerprint, credential?.ExpiresAt, feedCount, x.Version);


    private static string SafeConfiguration(JsonElement configuration)
    {
        if (configuration.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null) return "{}";
        if (configuration.ValueKind != JsonValueKind.Object) throw new ArgumentException("Integration configuration must be a JSON object.");
        EnsureNoSensitiveKeys(configuration); string json = configuration.GetRawText();
        if (json.Length > 32_000) throw new ArgumentException("Integration configuration cannot exceed 32000 characters.");
        return json;
    }

    private static void EnsureNoSensitiveKeys(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (IsSensitiveConfigurationKey(property.Name)) throw new ArgumentException($"Configuration property '{property.Name}' is sensitive; store it through a credential reference.");
                EnsureNoSensitiveKeys(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array) foreach (JsonElement item in element.EnumerateArray()) EnsureNoSensitiveKeys(item);
    }

    private static bool IsSensitiveConfigurationKey(string key)
    {
        string normalized = new(key.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        return SensitiveConfigurationKeys.Contains(key)
            || normalized is "token" or "password" or "secret" or "apikey" or "accesskey" or "privatekey" or "credential" or "credentials"
            || normalized.EndsWith("password", StringComparison.Ordinal)
            || normalized.EndsWith("secret", StringComparison.Ordinal)
            || normalized.EndsWith("apikey", StringComparison.Ordinal)
            || normalized.EndsWith("accesskey", StringComparison.Ordinal)
            || normalized.EndsWith("privatekey", StringComparison.Ordinal)
            || normalized.EndsWith("accesstoken", StringComparison.Ordinal)
            || normalized.EndsWith("refreshtoken", StringComparison.Ordinal)
            || normalized.EndsWith("authtoken", StringComparison.Ordinal)
            || normalized.EndsWith("bearertoken", StringComparison.Ordinal);
    }

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

    private sealed record ActiveCredentialProjection(Guid ConnectionId, int CredentialVersion, string Fingerprint, DateTimeOffset? ExpiresAt);

    private static readonly HashSet<string> SensitiveConfigurationKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "secret", "password", "token", "apikey", "api_key", "accesskey", "access_key",
        "privatekey", "private_key", "clientsecret", "client_secret", "credential", "credentials"
    };

}

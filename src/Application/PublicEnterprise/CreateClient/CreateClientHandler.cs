using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateClient;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;

public sealed class CreateClientHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context, TenantDatabaseRoutingScope routing) : IRequestHandler<CreateClientCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateClientCommand message, CancellationToken cancellationToken) => CreateClientAsync(message.request, db, scope, actor, tenant, context, routing, cancellationToken);

    private static async Task<UseCaseResult> CreateClientAsync(CreatePublicApiClientRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        Guid[] farmIds = request.FarmIds.Distinct().ToArray();
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        if (farmIds.Length > 0 && (!resolved.IsUnrestricted && !resolved.FarmIds!.IsSupersetOf(farmIds)
            || resolved.IsUnrestricted && await db.Farms.CountAsync(x => farmIds.Contains(x.Id), ct) != farmIds.Length)) return Bad("farm_scope_invalid", "Every API farm scope must be accessible to the caller.");
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow; Guid clientId = Id(request.Id);
            string? expiryError = await CredentialExpiryErrorAsync(null, request.ExpiresAt, now, db, routing, ct);
            if (expiryError is not null) return Bad("credential_rotation_policy_violation", expiryError);
            (string secret, string prefix, string hash) = Secret("mro_live");
            var client = new PublicApiClient(clientId, request.Name, request.Scopes, farmIds, now, actor.Subject);
            var credential = new PublicApiCredential(Guid.NewGuid(), clientId, 1, prefix, hash, now, request.ExpiresAt, now, actor.Subject);
            db.PublicApiClients.Add(client); db.PublicApiCredentials.Add(credential); AddManagementRecords(db, tenant, actor, "Created", client, context, now);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/public-api/clients/{client.Id}", new { client, credential = new { credential.Id, credential.KeyPrefix, credential.CredentialVersion, credential.ExpiresAt }, secret });
        }
        catch (ArgumentException e) { return Bad("api_client_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static (string Secret, string Prefix, string Hash) Secret(string prefix) { string raw = $"{prefix}_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}"; return (raw, raw[..Math.Min(16, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static void AddManagementRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, string action, PublicApiClient client, UseCaseContext context, DateTimeOffset now) { string json = JsonSerializer.Serialize(new { client.Id, client.Name, client.Scopes, client.FarmIds, client.Status }); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, nameof(PublicApiClient), client.Id, null, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"PublicApi.Client{action}", json, now)); }

    private static async Task<string?> CredentialExpiryErrorAsync(Guid? clientId, DateTimeOffset? expiresAt,
        DateTimeOffset issuedAt, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        ApiCredentialRotationPolicy? policy = clientId.HasValue
            ? await db.ApiCredentialRotationPolicies.AsNoTracking().Where(x => x.IsActive && x.PublicApiClientId == clientId).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(ct)
            : null;
        policy ??= await db.ApiCredentialRotationPolicies.AsNoTracking().Where(x => x.IsActive && x.PublicApiClientId == null).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(ct);
        if (policy is null) return null;
        if (policy.RequireExpiry && !expiresAt.HasValue) return "The active rotation policy requires an expiry date.";
        if (expiresAt > issuedAt.AddDays(policy.MaximumAgeDays)) return $"Credential lifetime cannot exceed {policy.MaximumAgeDays} days.";
        return null;
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}

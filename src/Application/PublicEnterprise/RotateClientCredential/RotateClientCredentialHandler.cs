using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.RotateClientCredential;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using PublicApiClientStatus = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClientStatus;
using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;
using PublicApiCredentialStatus = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredentialStatus;

public sealed class RotateClientCredentialHandler(IPlatformDbContext db, IRequestActorContext actor, TenantDatabaseRoutingScope routing) : IRequestHandler<RotateClientCredentialCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RotateClientCredentialCommand message, CancellationToken cancellationToken) => RotateClientCredentialAsync(message.clientId, message.request, db, actor, routing, cancellationToken);

    private static async Task<UseCaseResult> RotateClientCredentialAsync(Guid clientId, RotateCredentialRequest request, IPlatformDbContext db,
        IRequestActorContext actor, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        PublicApiClient? client = await db.PublicApiClients.SingleOrDefaultAsync(x => x.Id == clientId && x.Status != PublicApiClientStatus.Revoked, ct); if (client is null) return UseCaseResults.NotFound();
        PublicApiCredential? previous = await db.PublicApiCredentials.Where(x => x.ClientId == clientId && x.Status == PublicApiCredentialStatus.Active).OrderByDescending(x => x.CredentialVersion).FirstOrDefaultAsync(ct);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        string? expiryError = await CredentialExpiryErrorAsync(clientId, request.ExpiresAt, now, db, routing, ct);
        if (expiryError is not null) return Bad("credential_rotation_policy_violation", expiryError);
        (string secret, string prefix, string hash) = Secret("mro_live"); int version = (previous?.CredentialVersion ?? 0) + 1; Guid id = Guid.NewGuid();
        try { var credential = new PublicApiCredential(id, clientId, version, prefix, hash, now, request.ExpiresAt, now, actor.Subject, previous?.Id); previous?.Supersede(id, now); db.PublicApiCredentials.Add(credential); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { credential = new { credential.Id, credential.KeyPrefix, credential.CredentialVersion, credential.ExpiresAt }, secret }); }
        catch (ArgumentException e) { return Bad("api_credential_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static (string Secret, string Prefix, string Hash) Secret(string prefix) { string raw = $"{prefix}_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}"; return (raw, raw[..Math.Min(16, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

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

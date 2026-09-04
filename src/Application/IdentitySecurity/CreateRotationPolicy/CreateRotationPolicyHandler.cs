using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRotationPolicy;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;

public sealed class CreateRotationPolicyHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<CreateRotationPolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRotationPolicyCommand message, CancellationToken cancellationToken) => CreateRotationPolicyAsync(message.request, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateRotationPolicyAsync(CreateCredentialRotationPolicyRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, CancellationToken ct)
    {
        if (request.PublicApiClientId.HasValue && !await db.PublicApiClients.AnyAsync(x => x.Id == request.PublicApiClientId, ct)) return Bad("api_client_not_found", "The public API client does not exist.");
        using IDisposable control = routing.EnterControlPlane(); if (await db.ApiCredentialRotationPolicies.AnyAsync(x => x.PublicApiClientId == request.PublicApiClientId && x.IsActive, ct)) return Conflict("active_rotation_policy_exists", "Retire the active policy for this scope first.");
        try { var policy = new ApiCredentialRotationPolicy(Id(request.Id), request.PublicApiClientId, request.MaximumAgeDays, request.WarningDays, request.RequireExpiry, DateTimeOffset.UtcNow, actor.Subject); db.ApiCredentialRotationPolicies.Add(policy); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/security/credential-rotation-policies/{policy.Id}", policy); }
        catch (ArgumentException e) { return Bad("credential_rotation_policy_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

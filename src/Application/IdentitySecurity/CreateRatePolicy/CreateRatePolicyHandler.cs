using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRatePolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class CreateRatePolicyHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<CreateRatePolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRatePolicyCommand message, CancellationToken cancellationToken) => CreateRatePolicyAsync(message.request, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateRatePolicyAsync(CreateApiRateLimitPolicyRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, CancellationToken ct)
    {
        if (request.PublicApiClientId.HasValue && !await db.PublicApiClients.AnyAsync(x => x.Id == request.PublicApiClientId, ct)) return Bad("api_client_not_found", "The public API client does not exist.");
        using IDisposable control = routing.EnterControlPlane(); if (await db.ApiRateLimitPolicies.AnyAsync(x => x.PublicApiClientId == request.PublicApiClientId && x.IsActive, ct)) return Conflict("active_rate_policy_exists", "Retire the active policy for this scope first.");
        try { var policy = new ApiRateLimitPolicy(Id(request.Id), request.PublicApiClientId, request.RequestLimit, request.WindowSeconds, request.DailyQuota, DateTimeOffset.UtcNow, actor.Subject); db.ApiRateLimitPolicies.Add(policy); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/security/rate-limit-policies/{policy.Id}", policy); }
        catch (ArgumentException e) { return Bad("rate_limit_policy_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

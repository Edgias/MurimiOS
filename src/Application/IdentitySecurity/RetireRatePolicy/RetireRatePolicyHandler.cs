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
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.RetireRatePolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class RetireRatePolicyHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<RetireRatePolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireRatePolicyCommand message, CancellationToken cancellationToken) => RetireRatePolicyAsync(message.policyId, db, routing, cancellationToken);

    private static async Task<UseCaseResult> RetireRatePolicyAsync(Guid policyId, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); ApiRateLimitPolicy? policy = await db.ApiRateLimitPolicies.SingleOrDefaultAsync(x => x.Id == policyId, ct); if (policy is null) return UseCaseResults.NotFound(); try { policy.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(policy); } catch (InvalidOperationException e) { return Conflict("rate_limit_policy_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

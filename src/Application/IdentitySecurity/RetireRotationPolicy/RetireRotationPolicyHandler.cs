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
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.RetireRotationPolicy;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;

public sealed class RetireRotationPolicyHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<RetireRotationPolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireRotationPolicyCommand message, CancellationToken cancellationToken) => RetireRotationPolicyAsync(message.policyId, db, routing, cancellationToken);

    private static async Task<UseCaseResult> RetireRotationPolicyAsync(Guid policyId, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); ApiCredentialRotationPolicy? policy = await db.ApiCredentialRotationPolicies.SingleOrDefaultAsync(x => x.Id == policyId, ct); if (policy is null) return UseCaseResults.NotFound(); try { policy.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(policy); } catch (InvalidOperationException e) { return Conflict("credential_rotation_policy_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

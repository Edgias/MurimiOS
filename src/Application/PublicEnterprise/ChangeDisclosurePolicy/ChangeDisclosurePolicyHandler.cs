using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ChangeDisclosurePolicy;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;

public sealed class ChangeDisclosurePolicyHandler(IPlatformDbContext db) : IRequestHandler<ChangeDisclosurePolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeDisclosurePolicyCommand message, CancellationToken cancellationToken) => ChangeDisclosurePolicyAsync(message.policyId, message.command, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeDisclosurePolicyAsync(Guid policyId, string command, IPlatformDbContext db, CancellationToken ct)
    { RetailDisclosurePolicy? policy = await db.RetailDisclosurePolicies.SingleOrDefaultAsync(x => x.Id == policyId, ct); if (policy is null) return UseCaseResults.NotFound(); try { if (command == "publish") policy.Publish(DateTimeOffset.UtcNow); else policy.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(policy); } catch (InvalidOperationException e) { return Conflict("disclosure_policy_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

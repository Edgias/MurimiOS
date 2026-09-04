using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.DisableRetentionPolicy;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;

public sealed class DisableRetentionPolicyHandler(IPlatformDbContext db) : IRequestHandler<DisableRetentionPolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DisableRetentionPolicyCommand message, CancellationToken cancellationToken) => DisableRetentionPolicyAsync(message.policyId, db, cancellationToken);
    private static async Task<UseCaseResult> DisableRetentionPolicyAsync(Guid policyId, IPlatformDbContext db, CancellationToken ct)
    { AnalyticsRetentionPolicy? value = await db.AnalyticsRetentionPolicies.SingleOrDefaultAsync(x => x.Id == policyId, ct); if (value is null) return UseCaseResults.NotFound(); try { value.Disable(); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(value); } catch (InvalidOperationException e) { return Conflict("analytics_retention_transition_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

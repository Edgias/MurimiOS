using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.EffectiveEntitlements;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;

public sealed class EffectiveEntitlementsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<EffectiveEntitlementsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(EffectiveEntitlementsQuery message, CancellationToken cancellationToken) => EffectiveEntitlementsAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> EffectiveEntitlementsAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); DateTimeOffset now = DateTimeOffset.UtcNow;
        TenantSubscription? subscription = await CurrentSubscription(db, now, ct);
        if (subscription is null) return UseCaseResults.Ok(new { subscription = (object?)null, entitlements = Array.Empty<object>() });
        PlanEntitlement[] values = await db.PlanEntitlements.AsNoTracking().Where(x => x.PlanVersionId == subscription.PlanVersionId).OrderBy(x => x.Key).ToArrayAsync(ct);
        UsageMeterPeriod[] usage = await db.UsageMeterPeriods.AsNoTracking().Where(x => x.PeriodStartsAt == subscription.CurrentPeriodStartsAt && x.PeriodEndsAt == subscription.CurrentPeriodEndsAt).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { subscription, entitlements = values.Select(value => new { entitlement = value, consumed = usage.SingleOrDefault(x => x.MetricCode == value.Key)?.Quantity ?? 0, remaining = value.IncludedAmount.HasValue ? Math.Max(0, value.IncludedAmount.Value - (usage.SingleOrDefault(x => x.MetricCode == value.Key)?.Quantity ?? 0)) : (decimal?)null }) });
    }


    private static Task<TenantSubscription?> CurrentSubscription(IPlatformDbContext db, DateTimeOffset now, CancellationToken ct)
        => db.TenantSubscriptions.Where(x => (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing)
            && x.StartsAt <= now && x.CurrentPeriodEndsAt > now).OrderByDescending(x => x.StartsAt).FirstOrDefaultAsync(ct);

}

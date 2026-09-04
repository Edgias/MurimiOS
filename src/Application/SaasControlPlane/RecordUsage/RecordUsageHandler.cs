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
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.RecordUsage;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

using EntitlementEnforcement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementEnforcement;
using EntitlementKind = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementKind;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
using UsageMeterEvent = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterEvent;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;

public sealed class RecordUsageHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<RecordUsageCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RecordUsageCommand message, CancellationToken cancellationToken) => RecordUsageAsync(message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> RecordUsageAsync(RecordUsageRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        UsageMeterEvent? duplicate = await db.UsageMeterEvents.AsNoTracking().SingleOrDefaultAsync(x => x.IdempotencyKey == request.IdempotencyKey, ct);
        if (duplicate is not null) return UseCaseResults.Ok(new { usageEvent = duplicate, duplicate = true });
        DateTimeOffset now = DateTimeOffset.UtcNow; TenantSubscription? subscription = await CurrentSubscription(db, now, ct);
        if (subscription is null) return PaymentRequired("active_subscription_required", "Usage cannot be recorded without a current subscription.");
        string metric = request.MetricCode.Trim().ToLowerInvariant();
        PlanEntitlement? entitlement = await db.PlanEntitlements.AsNoTracking().SingleOrDefaultAsync(x => x.PlanVersionId == subscription.PlanVersionId && x.Key == metric, ct);
        if (entitlement is null) return UseCaseResults.Forbid();
        if (entitlement.Kind != EntitlementKind.Limit) return Bad("usage_metric_invalid", "Feature entitlements cannot accept metered quantities.");
        if (!entitlement.Unit.Equals(request.Unit.Trim(), StringComparison.OrdinalIgnoreCase)) return Bad("usage_unit_mismatch", "Usage must use the entitlement's configured unit.");
        UsageMeterPeriod? period = await db.UsageMeterPeriods.SingleOrDefaultAsync(x => x.MetricCode == metric
            && x.PeriodStartsAt == subscription.CurrentPeriodStartsAt && x.PeriodEndsAt == subscription.CurrentPeriodEndsAt, ct);
        decimal consumed = period?.Quantity ?? 0; decimal projected = consumed + request.Quantity;
        if (entitlement.Enforcement == EntitlementEnforcement.Hard && projected > entitlement.IncludedAmount)
            return UseCaseResults.Json(new { code = "entitlement_limit_exceeded", message = $"{metric} would exceed the included {entitlement.IncludedAmount} {entitlement.Unit} limit.", consumed, requested = request.Quantity, limit = entitlement.IncludedAmount }, statusCode: 429);
        try
        {
            string dimensions = request.Dimensions.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null ? "{}" : request.Dimensions.GetRawText();
            var usageEvent = new UsageMeterEvent(Id(request.Id), request.IdempotencyKey, metric, request.Quantity, request.Unit, request.OccurredAt ?? now, request.Source, dimensions);
            if (period is null) { period = new UsageMeterPeriod(Guid.NewGuid(), metric, request.Unit, subscription.CurrentPeriodStartsAt, subscription.CurrentPeriodEndsAt, request.Quantity, now); db.UsageMeterPeriods.Add(period); }
            else { if (!period.Unit.Equals(request.Unit.Trim(), StringComparison.OrdinalIgnoreCase)) return Bad("usage_unit_mismatch", "Usage must use the entitlement period's existing unit."); period.Add(request.Quantity, now); }
            db.UsageMeterEvents.Add(usageEvent); await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return UseCaseResults.Created($"/api/v1/saas/usage/{usageEvent.Id}", new { usageEvent, period, limitExceeded = entitlement.IncludedAmount.HasValue && period.Quantity > entitlement.IncludedAmount });
        }
        catch (ArgumentException e) { return Bad("usage_event_invalid", e.Message); }
    }


    private static Task<TenantSubscription?> CurrentSubscription(IPlatformDbContext db, DateTimeOffset now, CancellationToken ct)
        => db.TenantSubscriptions.Where(x => (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing)
            && x.StartsAt <= now && x.CurrentPeriodEndsAt > now).OrderByDescending(x => x.StartsAt).FirstOrDefaultAsync(ct);


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult PaymentRequired(string code, string message) => UseCaseResults.Json(new { code, message }, statusCode: 402);

}

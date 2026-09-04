using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreateSubscription;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;

public sealed class CreateSubscriptionHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<CreateSubscriptionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateSubscriptionCommand message, CancellationToken cancellationToken) => CreateSubscriptionAsync(message.request, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateSubscriptionAsync(CreateTenantSubscriptionRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        if (!await db.TenantAccounts.AnyAsync(ct)) return Conflict("tenant_account_required", "Provision the SaaS tenant account first.");
        SaasPlanVersion? plan = await db.SaasPlanVersions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.PlanVersionId, ct);
        if (plan?.Status != SaasPlanStatus.Published) return Bad("published_plan_required", "A subscription requires a published plan version.");
        if (await db.TenantSubscriptions.AnyAsync(x => x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing
            || x.Status == SubscriptionStatus.PastDue || x.Status == SubscriptionStatus.Suspended, ct))
            return Conflict("current_subscription_exists", "End the current subscription before assigning another plan.");
        try
        {
            var subscription = new TenantSubscription(Id(request.Id), plan.Id, request.StartsAt, request.CurrentPeriodEndsAt,
                request.TrialEndsAt, request.ProviderSubscriptionReference, DateTimeOffset.UtcNow, actor.Subject);
            db.TenantSubscriptions.Add(subscription); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/saas/subscriptions/{subscription.Id}", subscription);
        }
        catch (ArgumentException e) { return Bad("subscription_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

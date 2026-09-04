using EdgIQ.MurimiOS.Domain.TenantAccount;
using EdgIQ.MurimiOS.Domain.TenantSubscription;


using EdgIQ.MurimiOS.Application.Common.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;
using TenantAccountStatus = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccountStatus;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using IPlatformDbContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using TenantDatabaseRoutingScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.TenantDatabaseRoutingScope;

public sealed class SaasTenantLifecycleMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenant, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, TimeProvider timeProvider)
    {
        if (!tenant.IsResolved || IsControlPath(context.Request.Path))
        {
            await next(context);
            return;
        }

        TenantAccount? account;
        bool currentSubscription = false;
        DateTimeOffset now = timeProvider.GetUtcNow();
        using (routing.EnterControlPlane())
        {
            account = await db.TenantAccounts.AsNoTracking().SingleOrDefaultAsync(context.RequestAborted);
            if (account?.Status == TenantAccountStatus.Active)
            {
                currentSubscription = await db.TenantSubscriptions.AsNoTracking().AnyAsync(x =>
                    (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trialing)
                    && x.StartsAt <= now && x.CurrentPeriodEndsAt > now, context.RequestAborted);
            }
        }
        if (account is null)
        {
            // Backwards-compatible until an existing tenant is explicitly enrolled in the commercial control plane.
            await next(context);
            return;
        }

        if (account.Status != TenantAccountStatus.Active)
        {
            int status = account.Status == TenantAccountStatus.Closed
                ? StatusCodes.Status410Gone
                : StatusCodes.Status423Locked;
            await ProblemAsync(context, status, "tenant_not_active", "Tenant unavailable",
                $"The tenant account is {account.Status.ToString().ToLowerInvariant()}.");
            return;
        }

        if (!currentSubscription)
        {
            await ProblemAsync(context, StatusCodes.Status402PaymentRequired, "active_subscription_required",
                "Subscription required", "The tenant has no active subscription or trial for the current billing period.");
            return;
        }

        await next(context);
    }

    private static bool IsControlPath(PathString path) =>
        path.StartsWithSegments("/api/v1/saas")
        || path.StartsWithSegments("/health")
        || path.StartsWithSegments("/swagger")
        || path.StartsWithSegments("/openapi")
        || path.StartsWithSegments("/api/public/v1");

    private static Task ProblemAsync(HttpContext context, int status, string code, string title, string detail)
    {
        context.Response.StatusCode = status;
        return Results.Problem(statusCode: status, title: title, detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code }).ExecuteAsync(context);
    }
}

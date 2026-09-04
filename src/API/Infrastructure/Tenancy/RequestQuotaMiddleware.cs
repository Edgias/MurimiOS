

using EdgIQ.MurimiOS.Application.Common.Abstractions;


namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using IRequestQuotaStore = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestQuotaStore;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using RequestQuotaDecision = global::EdgIQ.MurimiOS.Application.Common.Abstractions.RequestQuotaDecision;

public sealed class RequestQuotaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenant, IRequestQuotaStore quotas, TimeProvider timeProvider)
    {
        if (!tenant.IsResolved || !context.Request.Path.StartsWithSegments("/api/v1"))
        { await next(context); return; }

        RequestQuotaDecision decision = await quotas.ConsumeTenantAsync(tenant.TenantId, timeProvider.GetUtcNow(), context.RequestAborted);
        context.Response.Headers["X-RateLimit-Remaining"] = decision.RequestRemaining.ToString();
        context.Response.Headers["X-DailyQuota-Remaining"] = decision.DailyRemaining.ToString();
        if (!decision.Allowed)
        {
            context.Response.Headers.RetryAfter = decision.RetryAfterSeconds.ToString();
            await Results.Json(new { code = "request_quota_exceeded", message = "The tenant request rate or daily quota has been exceeded.", retryAfterSeconds = decision.RetryAfterSeconds }, statusCode: StatusCodes.Status429TooManyRequests).ExecuteAsync(context);
            return;
        }
        await next(context);
    }
}

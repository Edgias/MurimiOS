using System.Diagnostics;
using EdgIQ.MurimiOS.Application.Common.Abstractions;




namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using IRequestActorContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestActorContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class RequestTelemetryMiddleware(RequestDelegate next, ILogger<RequestTelemetryMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenant, IRequestActorContext actor)
    {
        string module = ModuleFor(context.Request.Path);
        Activity.Current?.SetTag("murimios.module", module);
        if (tenant.IsResolved)
        {
            Activity.Current?.SetTag("tenant.id", tenant.TenantId);
            if (actor.ScopeOrganisationNodeId is { } nodeId)
                Activity.Current?.SetTag("org.node", nodeId);
        }

        using IDisposable? scope = tenant.IsResolved
            ? logger.BeginScope(new Dictionary<string, object>
            {
                ["TenantId"] = tenant.TenantId,
                ["Module"] = module,
                ["OrganisationNodeId"] = actor.ScopeOrganisationNodeId?.ToString() ?? string.Empty
            })
            : logger.BeginScope(new Dictionary<string, object> { ["Module"] = module });
        await next(context);
    }

    private static string ModuleFor(PathString path)
    {
        string value = path.Value?.ToLowerInvariant() ?? string.Empty;
        if (value.Contains("livestock")) return "livestock";
        if (value.Contains("notification")) return "notifications";
        if (value.Contains("evidence") || value.Contains("media")) return "evidence";
        if (value.Contains("sync")) return "sync";
        if (value.Contains("audit") || value.Contains("correction")) return "audit";
        if (value.Contains("report")) return "reporting";
        if (value.Contains("input") || value.Contains("stock")) return "inventory";
        if (value.Contains("farm") || value.Contains("field")) return "farm";
        return "platform";
    }
}

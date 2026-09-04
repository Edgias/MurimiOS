using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using ITenantDatabaseRouteResolver = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantDatabaseRouteResolver;

public sealed class TenantMigrationWriteGuardMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, HttpTenantContext tenant,
        ITenantDatabaseRouteResolver routes)
    {
        if (!tenant.IsResolved || IsRead(context.Request.Method) || IsMigrationControlPath(context.Request.Path))
        {
            await next(context);
            return;
        }
        if (!await routes.IsWriteBlockedAsync(tenant.TenantId, context.RequestAborted))
        {
            await next(context);
            return;
        }
        context.Response.StatusCode = StatusCodes.Status423Locked;
        await Results.Problem(statusCode: StatusCodes.Status423Locked, title: "Tenant migration in progress",
            detail: "Writes are temporarily locked while the tenant database is exported, validated, imported, and cut over.",
            extensions: new Dictionary<string, object?> { ["code"] = "tenant_migration_write_locked" })
            .ExecuteAsync(context);
    }

    private static bool IsRead(string method) => HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);
    private static bool IsMigrationControlPath(PathString path) => path.StartsWithSegments("/api/v1/enterprise/migration-jobs");
}

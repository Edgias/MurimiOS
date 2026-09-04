

using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;

using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using TenantDatabaseRoutingScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.TenantDatabaseRoutingScope;
using TenantDatabaseRegistry = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseRegistry;
using TenantDatabaseRouteResolver = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseRouteResolver;

public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ITenantContext tenantContext;
    private readonly TenantDatabaseRouteResolver? routes;
    private readonly TenantDatabaseRegistry? registry;
    private readonly TenantDatabaseRoutingScope? routingScope;

    public TenantConnectionInterceptor(ITenantContext tenantContext)
    {
        this.tenantContext = tenantContext;
    }

    public TenantConnectionInterceptor(ITenantContext tenantContext, TenantDatabaseRouteResolver routes,
        TenantDatabaseRegistry registry, TenantDatabaseRoutingScope routingScope)
    {
        this.tenantContext = tenantContext;
        this.routes = routes;
        this.registry = registry;
        this.routingScope = routingScope;
    }
    public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData,
        InterceptionResult result)
    {
        Route(connection);
        return result;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
        ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
    {
        await RouteAsync(connection, cancellationToken);
        return result;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenant(connection);
    }

    public override Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        return SetTenantAsync(connection, cancellationToken);
    }

    private void SetTenant(DbConnection connection)
    {
        if (!tenantContext.IsResolved)
        {
            return;
        }

        using DbCommand command = connection.CreateCommand();
        command.CommandText = "select set_config('app.tenant_id', @tenant_id, false)";
        DbParameter parameter = command.CreateParameter();
        parameter.ParameterName = "tenant_id";
        parameter.Value = tenantContext.TenantId.ToString();
        command.Parameters.Add(parameter);
        command.ExecuteNonQuery();
    }

    private void Route(DbConnection connection)
    {
        if (connection.State != System.Data.ConnectionState.Closed || registry is null || routes is null || routingScope is null) return;
        connection.ConnectionString = routingScope.UseControlDatabase || !tenantContext.IsResolved
            ? registry.ControlConnectionString
            : routes.ResolveConnectionString(tenantContext.TenantId);
    }

    private async Task RouteAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        if (connection.State != System.Data.ConnectionState.Closed || registry is null || routes is null || routingScope is null) return;
        connection.ConnectionString = routingScope.UseControlDatabase || !tenantContext.IsResolved
            ? registry.ControlConnectionString
            : await routes.ResolveConnectionStringAsync(tenantContext.TenantId, cancellationToken);
    }

    private async Task SetTenantAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
        {
            return;
        }

        await using DbCommand command = connection.CreateCommand();
        command.CommandText = "select set_config('app.tenant_id', @tenant_id, false)";
        DbParameter parameter = command.CreateParameter();
        parameter.ParameterName = "tenant_id";
        parameter.Value = tenantContext.TenantId.ToString();
        command.Parameters.Add(parameter);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
public static class TenantRoutingServiceCollectionExtensions
{
    public static IServiceCollection AddMurimiOSTenantRouting(this IServiceCollection services, IConfiguration configuration, string platformConnection)
    {
services.AddOptions<TenantDatabaseOptions>()
    .Bind(configuration.GetSection("TenantDatabases"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Tenant database configuration is invalid.")
    .ValidateOnStart();
services.AddSingleton(services => new TenantDatabaseRegistry(
    services.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantDatabaseOptions>>(),
    services.GetRequiredService<IConfiguration>(), platformConnection));
services.AddSingleton<TenantDatabaseRouteResolver>();
services.AddScoped<TenantDatabaseRoutingScope>();

        return services;
    }
}

using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Storage.Compliance;
using EdgIQ.MurimiOS.Application.Common.Compliance;
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
public static class ComplianceServiceCollectionExtensions
{
    public static IServiceCollection AddMurimiOSCompliance(this IServiceCollection services, IConfiguration configuration)
    {
services.AddSingleton<ComplianceStandardPackCatalog>();
services.AddScoped<ComplianceStandardPackInstaller>();
services.AddScoped<ComplianceEvaluationEngine>();
services.AddOptions<ComplianceAuditPackOptions>()
    .Bind(configuration.GetSection("ComplianceAuditPacks"))
    .Validate(options => { try { options.ValidateAndGetKey(); return true; } catch { return false; } },
        "Compliance audit-pack signing and size configuration is invalid.")
    .ValidateOnStart();
services.AddSingleton<ComplianceAuditPackSignatureService>();
services.AddScoped<ComplianceAuditPackGenerator>();

        return services;
    }
}

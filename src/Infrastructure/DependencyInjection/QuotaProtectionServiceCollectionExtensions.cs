using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Infrastructure.Identity;
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
public static class QuotaProtectionServiceCollectionExtensions
{
    public static IServiceCollection AddMurimiOSQuotaProtection(this IServiceCollection services, IConfiguration configuration)
    {
services.AddOptions<RequestQuotaOptions>()
    .Bind(configuration.GetSection("RequestQuotas"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Request quota defaults are invalid.")
    .ValidateOnStart();
services.AddSingleton<RequestQuotaStore>();

        return services;
    }
}

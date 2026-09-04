using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock;
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
public static class LivestockAdaptersServiceCollectionExtensions
{
    public static IServiceCollection AddMurimiOSLivestockAdapters(this IServiceCollection services)
    {
services.AddHttpClient<ScotEidMovementAdapter>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MurimiOS/1.0");
});
services.AddHttpClient<IcarAdeExchangeAdapter>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MurimiOS/1.0");
});


        return services;
    }
}

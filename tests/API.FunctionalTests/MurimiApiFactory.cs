using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class MurimiApiFactory : WebApplicationFactory<Program>
{
    public const string DefaultConnection =
        "Host=localhost;Port=55432;Database=murimios;Username=murimios_app;Password=murimios-app-dev;Include Error Detail=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:MurimiOS"] = Environment.GetEnvironmentVariable("MURIMIOS_TEST_CONNECTION") ?? DefaultConnection,
                ["TenantDatabases:ControlDatabaseKey"] = "primary",
                ["TenantDatabases:Connections:primary"] = "MurimiOS",
                ["Authentication:AllowDevelopmentTenantHeader"] = "true",
                ["Authentication:DevelopmentSubject"] = "integration-bootstrap",
                ["Authentication:DevelopmentPlatformOperator"] = "true",
                ["Authentication:RequireHttpsMetadata"] = "false",
                ["Storage:Provider"] = "Local",
                ["Storage:MediaRoot"] = Path.Combine(Path.GetTempPath(), "murimios-integration-media"),
                ["Storage:DataProtectionKeyRoot"] = Path.Combine(Path.GetTempPath(), "murimios-integration-keys"),
                ["MediaScanning:Provider"] = "Development",
                ["OpenTelemetry:OtlpEndpoint"] = string.Empty
            }));
    }

    public HttpClient CreateTenantClient(Guid tenantId)
    {
        HttpClient client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());
        client.DefaultRequestHeaders.Add("X-Development-Subject", "integration-bootstrap");
        return client;
    }

    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("MURIMIOS_TEST_CONNECTION") ?? DefaultConnection;
}

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class ApiSecurityAndContractTests
{
    [IntegrationFact]
    public async Task Protected_api_rejects_anonymous_requests_but_liveness_is_public()
    {
        await using var factory = new MurimiApiFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        HttpResponseMessage protectedResponse = await client.GetAsync("/api/v1/platform");
        HttpResponseMessage healthResponse = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);
    }

    [IntegrationFact]
    public async Task Development_header_authentication_is_disabled_in_production()
    {
        await using var factory = new ProductionSecurityFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        HttpResponseMessage response = await client.GetAsync("/api/v1/platform");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [IntegrationFact]
    public async Task Openapi_contract_contains_scheduled_analytics_surface_and_no_manual_run_operation()
    {
        await using var factory = new MurimiApiFactory();
        using HttpClient client = factory.CreateClient();

        using JsonDocument contract = JsonDocument.Parse(await client.GetStringAsync("/swagger/v1/swagger.json"));
        JsonElement paths = contract.RootElement.GetProperty("paths");

        Assert.True(paths.TryGetProperty("/api/v1/analytics/farms/{farmId}/schedules", out JsonElement schedules));
        Assert.True(schedules.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty("/api/v1/analytics/retention-policies", out JsonElement retention));
        Assert.True(retention.TryGetProperty("get", out _));
        Assert.True(retention.TryGetProperty("post", out _));
        Assert.True(paths.TryGetProperty("/api/v1/analytics/projections/{runId}/cancel", out _));
        Assert.False(paths.TryGetProperty("/api/v1/analytics/projections/{runId}/run", out _));
    }

    [IntegrationFact]
    public async Task Tenant_header_cannot_read_another_tenants_farm()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantA = Guid.NewGuid();
        Guid tenantB = Guid.NewGuid();
        using HttpClient clientA = factory.CreateTenantClient(tenantA);
        using HttpClient clientB = factory.CreateTenantClient(tenantB);
        Guid farmId = Guid.NewGuid();

        HttpResponseMessage created = await clientA.PostAsJsonAsync("/api/v1/farms", new
        {
            id = farmId,
            name = "RLS Contract Farm",
            code = $"R{farmId:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        });
        HttpResponseMessage crossTenant = await clientB.GetAsync($"/api/v1/analytics/farms/{farmId}/dashboard");

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, crossTenant.StatusCode);
    }

    private sealed class ProductionSecurityFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.UseSetting("Storage:Provider", "Local");
            builder.UseSetting("Storage:MediaRoot", Path.Combine(Path.GetTempPath(), "murimios-production-security-media"));
            builder.UseSetting("Storage:DataProtectionKeyRoot", Path.Combine(Path.GetTempPath(), "murimios-production-security-keys"));
            builder.UseSetting("MediaScanning:Provider", "ClamAv");
            builder.UseSetting("Authentication:AllowDevelopmentTenantHeader", "true");
            builder.UseSetting("Authentication:Authority", "https://identity.integration.invalid");
            builder.UseSetting("Authentication:Audience", "murimios-api");
            builder.UseSetting("Authentication:RequireHttpsMetadata", "true");
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MurimiOS"] = MurimiApiFactory.ConnectionString,
                    ["TenantDatabases:ControlDatabaseKey"] = "primary",
                    ["TenantDatabases:Connections:primary"] = "MurimiOS",
                    ["Authentication:AllowDevelopmentTenantHeader"] = "true",
                    ["Authentication:Authority"] = "https://identity.integration.invalid",
                    ["Authentication:Audience"] = "murimios-api",
                    ["Authentication:RequireHttpsMetadata"] = "true",
                    ["Authentication:Security:RequireMfaForWrites"] = "true",
                    ["Authentication:Security:MfaClaimType"] = "amr",
                    ["Authentication:Security:MfaAcceptedValues:0"] = "mfa",
                    ["ComplianceAuditPacks:SigningKeyBase64"] = "aW50ZWdyYXRpb24tY29tcGxpYW5jZS1hdWRpdC1wYWNrLXNpZ25pbmcta2V5LTAx",
                    ["ComplianceAuditPacks:SigningKeyId"] = "integration-2026-01",
                    ["Storage:Provider"] = "Local",
                    ["Storage:S3:AccessKey"] = "integration-test",
                    ["Storage:S3:SecretKey"] = "integration-test-secret",
                    ["Storage:S3:Bucket"] = "integration-test",
                    ["Storage:MediaRoot"] = Path.Combine(Path.GetTempPath(), "murimios-production-security-media"),
                    ["Storage:DataProtectionKeyRoot"] = Path.Combine(Path.GetTempPath(), "murimios-production-security-keys"),
                    ["MediaScanning:Provider"] = "ClamAv",
                    ["OpenTelemetry:OtlpEndpoint"] = string.Empty
                }));
        }
    }
}

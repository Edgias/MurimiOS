using System.Text.Json.Nodes;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class LegacyContractTests
{
    [Fact]
    public async Task Opt_in_legacy_endpoints_retain_typed_response_metadata()
    {
        await using var original = new MurimiApiFactory();
        await using var factory = original.WithWebHostBuilder(builder =>
        {
            // Host settings are available when top-level Program reads this flag;
            // ConfigureAppConfiguration alone runs too late for that branch.
            builder.UseSetting("Features:LegacyEndpointsEnabled", "true");
            // The pre-existing legacy host lacks MediatR registration. Supply it
            // only in this metadata test; do not change production behavior.
            builder.ConfigureServices(services => services.AddScoped<IMediator>(provider => new Mediator(provider)));
        });
        using var client = factory.CreateClient();
        var document = JsonNode.Parse(await client.GetStringAsync("/swagger/v1/swagger.json"))!;
        var paths = document["paths"]!;
        Assert.True(paths["/crop-units"] is not null,
            $"Legacy flag: {factory.Services.GetRequiredService<IConfiguration>()["Features:LegacyEndpointsEnabled"]}; paths: {string.Join(", ", paths.AsObject().Select(p => p.Key).Where(p => p.Contains("crop", StringComparison.OrdinalIgnoreCase) || p.Contains("bin", StringComparison.OrdinalIgnoreCase)))}");
        Assert.NotNull(paths["/crop-units"]?["get"]?["responses"]?["200"]?["content"]);
        Assert.NotNull(paths["/crop-units/{id}"]?["get"]?["responses"]?["404"]);
        Assert.NotNull(paths["/crop-units"]?["post"]?["responses"]?["201"]?["content"]);
        Assert.NotNull(paths["/bins"]?["get"]?["responses"]?["200"]?["content"]);
        Assert.NotNull(paths["/bins/{id}"]?["delete"]?["responses"]?["404"]);
    }
}

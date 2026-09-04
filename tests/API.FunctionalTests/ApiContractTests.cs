using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.DependencyInjection;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using UseCaseResult = global::EdgIQ.MurimiOS.Application.Common.Results.UseCaseResult;

/// <summary>Host-only contract checks: no database or external provider calls.</summary>
public sealed class ApiContractTests
{
    [Fact]
    public async Task Production_host_resolves_command_and_query_handlers_and_dispatches_a_query()
    {
        await using var factory = new EdgIQ.MurimiOS.API.FunctionalTests.MurimiApiFactory();
        using var scope = factory.Services.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ISender>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IPublisher>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IDispatcher>());
        Assert.IsType<EdgIQ.MurimiOS.Application.Audit.List.ListHandler>(scope.ServiceProvider.GetRequiredService<
            IRequestHandler<EdgIQ.MurimiOS.Application.Audit.List.ListQuery, UseCaseResult>>());
        Assert.IsType<EdgIQ.MurimiOS.Application.Organisation.Create.CreateHandler>(scope.ServiceProvider.GetRequiredService<
            IRequestHandler<EdgIQ.MurimiOS.Application.Organisation.Create.CreateCommand, UseCaseResult>>());
        UseCaseResult result = await scope.ServiceProvider.GetRequiredService<ISender>().Send(
            new EdgIQ.MurimiOS.Application.Access.GetAccessMe.GetAccessMeQuery(), CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task OpenApi_contract_remains_unchanged()
    {
        await using var factory = new EdgIQ.MurimiOS.API.FunctionalTests.MurimiApiFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
        JsonNode actual = Normalize(JsonNode.Parse(await response.Content.ReadAsStringAsync())!);
        string? capture = Environment.GetEnvironmentVariable("MURIMIOS_CAPTURE_API_CONTRACT");
        if (capture == "1")
        {
            string root = FindRepository();
            string path = Path.Combine(root, "tests", "API.FunctionalTests", "Contracts", "platform-openapi.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, actual.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            return;
        }
        string expectedPath = Path.Combine(AppContext.BaseDirectory, "Contracts", "platform-openapi.json");
        Assert.True(File.Exists(expectedPath), "Capture and review the baseline before running this contract test.");
        JsonNode expected = JsonNode.Parse(await File.ReadAllTextAsync(expectedPath))!;
        Assert.True(JsonNode.DeepEquals(expected, actual), "The OpenAPI contract changed. Inspect routes, input schemas and response metadata; do not update the baseline merely to pass.");
    }

    private static JsonNode Normalize(JsonNode value) => value switch
    {
        JsonObject obj => new JsonObject(obj.OrderBy(p => p.Key, StringComparer.Ordinal)
            .Select(p => KeyValuePair.Create(p.Key, p.Value is null ? null : Normalize(p.Value)))),
        JsonArray array => new JsonArray(array.Select(v => v is null ? null : Normalize(v)).ToArray()),
        _ => value.DeepClone()
    };

    private static string FindRepository()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MurimiOS.sln"))) directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("Repository root not found.");
    }
}

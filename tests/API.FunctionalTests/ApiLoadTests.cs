using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class ApiLoadTests
{
    [LoadFact]
    public async Task Authenticated_platform_contract_handles_bounded_parallel_load()
    {
        await using var factory = new MurimiApiFactory();
        using HttpClient client = factory.CreateTenantClient(Guid.NewGuid());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/platform")).StatusCode);
        var timings = new ConcurrentBag<double>();

        Task[] callers = Enumerable.Range(0, 20).Select(async _ =>
        {
            for (int request = 0; request < 10; request++)
            {
                long started = Stopwatch.GetTimestamp();
                HttpResponseMessage response = await client.GetAsync("/api/v1/platform");
                timings.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }).ToArray();
        await Task.WhenAll(callers);

        double[] ordered = timings.Order().ToArray();
        double p95 = ordered[(int)Math.Ceiling(ordered.Length * 0.95) - 1];
        Assert.Equal(200, ordered.Length);
        Assert.True(p95 < 2_000, $"Expected p95 below 2000 ms but measured {p95:F1} ms.");
    }
}

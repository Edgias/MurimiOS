using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;

using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using IProviderAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IProviderAdapter;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using ProviderSpatialTarget = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderSpatialTarget;
using SatelliteStatisticValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SatelliteStatisticValue;

public sealed class SentinelHubSatelliteAdapter(HttpClient httpClient) : IProviderAdapter
{
    private static readonly ConcurrentDictionary<string, CachedToken> Tokens = new(StringComparer.Ordinal);
    private const string DefaultTokenEndpoint = "https://identity.dataspace.copernicus.eu/auth/realms/CDSE/protocol/openid-connect/token";
    private const string DefaultStatisticsEndpoint = "https://sh.dataspace.copernicus.eu/api/v1/statistics";
    private const string Evalscript = """
        //VERSION=3
        function setup() {
          return { input: [{ bands: ["B03","B04","B05","B08","B11","SCL","dataMask"] }],
                   output: [{ id:"ndvi", bands:1, sampleType:"FLOAT32" },
                            { id:"ndre", bands:1, sampleType:"FLOAT32" },
                            { id:"ndmi", bands:1, sampleType:"FLOAT32" },
                            { id:"dataMask", bands:1 }] };
        }
        function evaluatePixel(s) {
          let clear = s.dataMask && ![3,8,9,10,11].includes(s.SCL);
          return { ndvi:[clear ? index(s.B08,s.B04) : NaN],
                   ndre:[clear ? index(s.B08,s.B05) : NaN],
                   ndmi:[clear ? index(s.B08,s.B11) : NaN], dataMask:[clear ? 1 : 0] };
        }
        """;

    public string ProviderCode => "sentinel-hub";
    public IntegrationFeedType FeedType => IntegrationFeedType.Satellite;

    public async Task<ProviderIngestionResult> IngestAsync(ProviderIngestionContext context, CancellationToken cancellationToken)
    {
        ProviderSpatialTarget target = context.SpatialTarget
            ?? throw new ProviderAdapterException("satellite_target_missing", "A satellite feed requires a field boundary.", false);
        if (target.FieldId is null || target.BoundaryVersionId is null || string.IsNullOrWhiteSpace(target.BoundaryGeoJson))
            throw new ProviderAdapterException("satellite_boundary_missing", "A satellite feed requires a current field boundary.", false);

        JsonElement connection = ProviderJson.Object(context.ConnectionConfigurationJson, "Connection configuration");
        JsonElement feed = ProviderJson.Object(context.FeedConfigurationJson, "Feed configuration");
        JsonElement credential = ProviderJson.Object(context.CredentialSecret ?? "{}", "Sentinel Hub credential");
        string clientId = credential.String("clientId") ?? throw new ProviderAdapterException("credential_invalid", "Sentinel Hub clientId is required.", false);
        string clientSecret = credential.String("clientSecret") ?? throw new ProviderAdapterException("credential_invalid", "Sentinel Hub clientSecret is required.", false);
        string tokenEndpoint = connection.String("tokenEndpoint") ?? DefaultTokenEndpoint;
        string statisticsEndpoint = connection.String("statisticsEndpoint") ?? DefaultStatisticsEndpoint;
        string collection = feed.String("collection") ?? "sentinel-2-l2a";
        string interval = feed.String("aggregationInterval") ?? "P5D";
        int lookbackDays = Math.Clamp(feed.Int32("lookbackDays", 30), 1, 366);
        int maxCloudCoverage = Math.Clamp(feed.Int32("maxCloudCoverage", 80), 0, 100);
        DateTimeOffset to = (context.WindowEnd ?? context.Now).ToUniversalTime();
        DateTimeOffset from = (context.WindowStart ?? to.AddDays(-lookbackDays)).ToUniversalTime();
        string token = await GetTokenAsync(clientId, clientSecret, tokenEndpoint, context.Now, cancellationToken);
        using JsonDocument geometry = JsonDocument.Parse(target.BoundaryGeoJson);
        var requestBody = new
        {
            input = new
            {
                bounds = new { geometry = geometry.RootElement.Clone() },
                data = new[] { new { type = collection, dataFilter = new { maxCloudCoverage } } }
            },
            aggregation = new
            {
                timeRange = new { from = from.ToString("O"), to = to.ToString("O") },
                aggregationInterval = new { of = interval },
                evalscript = Evalscript,
                resx = feed.Int32("resolutionMetres", 10),
                resy = feed.Int32("resolutionMetres", 10)
            }
        };
        using var request = new HttpRequestMessage(HttpMethod.Post, statisticsEndpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        string payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure(response.StatusCode, payload);

        using JsonDocument document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("data", out JsonElement intervals) || intervals.ValueKind != JsonValueKind.Array)
            throw new ProviderAdapterException("sentinel_response_invalid", "Sentinel Hub returned no statistical intervals.", true, (int)response.StatusCode);

        var records = new List<ProviderRecord>();
        foreach (JsonElement item in intervals.EnumerateArray())
        {
            if (!item.TryGetProperty("interval", out JsonElement period)
                || !DateTimeOffset.TryParse(period.String("from"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset acquiredAt)
                || !item.TryGetProperty("outputs", out JsonElement outputs)) continue;
            foreach ((string outputName, VegetationIndex index) in new[] { ("ndvi", VegetationIndex.Ndvi), ("ndre", VegetationIndex.Ndre), ("ndmi", VegetationIndex.Ndmi) })
            {
                if (!TryStatistics(outputs, outputName, out decimal mean, out decimal minimum, out decimal maximum, out decimal deviation, out decimal validPercent)) continue;
                decimal cloudPercent = decimal.Clamp(100m - validPercent, 0m, 100m);
                string externalId = $"{context.ExternalSourceId}:{acquiredAt:yyyyMMddHHmm}:{outputName}";
                string recordPayload = JsonSerializer.Serialize(new { interval = period.Clone(), output = outputName, statistics = outputs.GetProperty(outputName).Clone() });
                records.Add(new ProviderRecord(externalId, acquiredAt, null, recordPayload,
                    new SatelliteStatisticValue(index, null, mean, minimum, maximum, deviation, validPercent, cloudPercent)));
            }
        }
        DateTimeOffset? latest = records.Count == 0 ? null : records.Max(x => x.ObservedAt);
        return new ProviderIngestionResult(records, JsonSerializer.Serialize(new { latestAcquisitionAt = latest, boundaryVersionId = target.BoundaryVersionId }), (int)response.StatusCode);
    }

    private async Task<string> GetTokenAsync(string clientId, string secret, string endpoint, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (Tokens.TryGetValue(clientId, out CachedToken? cached) && cached.ExpiresAt > now.AddMinutes(1)) return cached.Value;
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials", ["client_id"] = clientId, ["client_secret"] = secret
        });
        using HttpResponseMessage response = await httpClient.PostAsync(endpoint, content, cancellationToken);
        string payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure(response.StatusCode, payload, "sentinel_auth_failed");
        using JsonDocument document = JsonDocument.Parse(payload);
        string token = document.RootElement.String("access_token") ?? throw new ProviderAdapterException("sentinel_auth_invalid", "The token response contained no access token.", true, (int)response.StatusCode);
        int expiresIn = document.RootElement.Int32("expires_in", 600);
        Tokens[clientId] = new CachedToken(token, now.AddSeconds(Math.Max(60, expiresIn)));
        return token;
    }

    private static bool TryStatistics(JsonElement outputs, string name, out decimal mean, out decimal minimum, out decimal maximum, out decimal deviation, out decimal validPercent)
    {
        mean = minimum = maximum = deviation = validPercent = 0;
        if (!outputs.TryGetProperty(name, out JsonElement output) || !output.TryGetProperty("bands", out JsonElement bands)) return false;
        JsonElement band = bands.EnumerateObject().Select(x => x.Value).FirstOrDefault();
        if (band.ValueKind != JsonValueKind.Object || !band.TryGetProperty("stats", out JsonElement stats)) return false;
        decimal? value = stats.Decimal("mean");
        if (value is null) return false;
        mean = value.Value; minimum = stats.Decimal("min") ?? mean; maximum = stats.Decimal("max") ?? mean;
        deviation = Math.Max(0m, stats.Decimal("stDev") ?? 0m);
        decimal samples = stats.Decimal("sampleCount") ?? 0m;
        decimal noData = stats.Decimal("noDataCount") ?? 0m;
        validPercent = samples + noData <= 0 ? 100m : decimal.Clamp(samples / (samples + noData) * 100m, 0m, 100m);
        return minimum <= mean && mean <= maximum;
    }

    private static ProviderAdapterException Failure(HttpStatusCode status, string payload, string code = "sentinel_request_failed")
    {
        bool retriable = (int)status is 408 or 425 or 429 || (int)status >= 500;
        string message = payload.Length > 1_000 ? payload[..1_000] : payload;
        return new ProviderAdapterException(code, $"Sentinel Hub returned {(int)status}: {message}", retriable, (int)status);
    }

    private sealed record CachedToken(string Value, DateTimeOffset ExpiresAt);
}

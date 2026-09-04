using System.Net;
using System.Text;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Infrastructure.Integrations;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using EuPesticidesRegulatoryAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.EuPesticidesRegulatoryAdapter;
using OpenMeteoWeatherAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.OpenMeteoWeatherAdapter;
using SentinelHubSatelliteAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.SentinelHubSatelliteAdapter;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderSpatialTarget = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderSpatialTarget;
using RegulatoryCatalogueItemValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.RegulatoryCatalogueItemValue;
using SatelliteStatisticValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SatelliteStatisticValue;
using WeatherForecastValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherForecastValue;
using WeatherObservationValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherObservationValue;

public sealed class ProviderAdapterTests
{
    [Fact]
    public async Task OpenMeteo_maps_past_and_future_hourly_values()
    {
        var handler = new QueueHandler(Json("""
            {"hourly":{"time":["2026-08-31T09:00","2026-08-31T11:00"],"temperature_2m":[20,24],
            "relative_humidity_2m":[70,60],"precipitation":[1,0],"precipitation_probability":[80,10],
            "wind_speed_10m":[2,3],"wind_direction_10m":[90,120],"surface_pressure":[900,901],
            "soil_temperature_0cm":[21,25],"soil_moisture_0_to_1cm":[0.3,0.25]}}
            """));
        var adapter = new OpenMeteoWeatherAdapter(new HttpClient(handler));
        ProviderIngestionResult result = await adapter.IngestAsync(Context(IntegrationFeedType.Weather,
            new ProviderSpatialTarget(null, null, -17.82m, 31.05m, null), now: new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero)), default);

        Assert.IsType<WeatherObservationValue>(result.Records[0].Normalized);
        Assert.IsType<WeatherForecastValue>(result.Records[1].Normalized);
        Assert.Equal(30m, ((WeatherObservationValue)result.Records[0].Normalized).SoilMoisturePercent);
        Assert.Contains("wind_speed_unit=ms", handler.Requests.Single().RequestUri!.Query);
    }

    [Fact]
    public async Task SentinelHub_authenticates_and_maps_boundary_statistics()
    {
        var handler = new QueueHandler(
            Json("{\"access_token\":\"token\",\"expires_in\":600}"),
            Json("""
                {"data":[{"interval":{"from":"2026-08-25T00:00:00Z","to":"2026-08-30T00:00:00Z"},"outputs":{
                  "ndvi":{"bands":{"B0":{"stats":{"min":0.1,"max":0.8,"mean":0.5,"stDev":0.1,"sampleCount":90,"noDataCount":10}}}},
                  "ndre":{"bands":{"B0":{"stats":{"min":0.1,"max":0.6,"mean":0.3,"stDev":0.08,"sampleCount":90,"noDataCount":10}}}},
                  "ndmi":{"bands":{"B0":{"stats":{"min":-0.2,"max":0.5,"mean":0.2,"stDev":0.09,"sampleCount":90,"noDataCount":10}}}}
                }}]}
                """));
        var adapter = new SentinelHubSatelliteAdapter(new HttpClient(handler));
        ProviderIngestionContext context = Context(IntegrationFeedType.Satellite,
            new ProviderSpatialTarget(Guid.NewGuid(), Guid.NewGuid(), -17.82m, 31.05m,
                "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[31,-18],[32,-18],[32,-17],[31,-18]]]]}"),
            credential: "{\"clientId\":\"client\",\"clientSecret\":\"secret\"}");

        ProviderIngestionResult result = await adapter.IngestAsync(context, default);

        Assert.Equal(3, result.Records.Count);
        var ndvi = Assert.IsType<SatelliteStatisticValue>(result.Records[0].Normalized);
        Assert.Equal(VegetationIndex.Ndvi, ndvi.Index);
        Assert.Equal(90m, ndvi.ValidPixelPercent);
        Assert.Equal("Bearer", handler.Requests[1].Headers.Authorization!.Scheme);
    }

    [Fact]
    public async Task EuPesticides_maps_active_substances_to_versioned_catalogue_items()
    {
        var response = Json("""
            {"items":[{"id":"AS-1","name":"Example substance","approvalStatus":"Approved",
            "approvalDate":"2025-01-01","expirationDate":"2030-01-01","lowRisk":true}]}
            """);
        response.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"v42\"");
        var adapter = new EuPesticidesRegulatoryAdapter(new HttpClient(new QueueHandler(response)));

        ProviderIngestionResult result = await adapter.IngestAsync(Context(IntegrationFeedType.Regulatory, null), default);

        var item = Assert.IsType<RegulatoryCatalogueItemValue>(Assert.Single(result.Records).Normalized);
        Assert.Equal("v42", item.CatalogueVersion);
        Assert.Equal("AS-1", item.Code);
        Assert.Contains("Approved", item.AttributesJson);
    }

    private static ProviderIngestionContext Context(IntegrationFeedType type, ProviderSpatialTarget? target,
        DateTimeOffset? now = null, string? credential = null) => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        type, target?.FieldId is null ? IntegrationTargetType.Farm : IntegrationTargetType.Field, target?.FieldId, "source", "{}", "{}", null,
        credential, null, null, now ?? new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero), target);

    private static HttpResponseMessage Json(string body) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json")
    };

    private sealed class QueueHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> queue = new(responses);
        public List<HttpRequestMessage> Requests { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(queue.Dequeue());
        }
    }
}

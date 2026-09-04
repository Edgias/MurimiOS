using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IProviderAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IProviderAdapter;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderNormalizedRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderNormalizedRecord;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using ProviderSpatialTarget = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderSpatialTarget;
using WeatherForecastValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherForecastValue;
using WeatherObservationValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherObservationValue;

public sealed class OpenMeteoWeatherAdapter(HttpClient httpClient) : IProviderAdapter
{
    private static readonly string[] Variables =
    [
        "temperature_2m", "relative_humidity_2m", "precipitation", "precipitation_probability",
        "wind_speed_10m", "wind_direction_10m", "surface_pressure", "soil_temperature_0cm", "soil_moisture_0_to_1cm"
    ];

    public string ProviderCode => "open-meteo";
    public IntegrationFeedType FeedType => IntegrationFeedType.Weather;

    public async Task<ProviderIngestionResult> IngestAsync(ProviderIngestionContext context, CancellationToken cancellationToken)
    {
        ProviderSpatialTarget target = context.SpatialTarget
            ?? throw new ProviderAdapterException("weather_location_missing", "A weather feed requires a latitude and longitude.", false);
        JsonElement connection = ProviderJson.Object(context.ConnectionConfigurationJson, "Connection configuration");
        JsonElement feed = ProviderJson.Object(context.FeedConfigurationJson, "Feed configuration");
        string endpoint = connection.String("endpoint")?.TrimEnd('/') ?? "https://api.open-meteo.com/v1/forecast";
        int pastDays = Math.Clamp(feed.Int32("pastDays", 2), 0, 92);
        int forecastDays = Math.Clamp(feed.Int32("forecastDays", 7), 1, 16);
        string uri = $"{endpoint}?latitude={target.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={target.Longitude.ToString(CultureInfo.InvariantCulture)}&hourly={string.Join(',', Variables)}&past_days={pastDays}&forecast_days={forecastDays}&timezone=UTC&wind_speed_unit=ms";

        using HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
        string payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw Failure("open_meteo_request_failed", response.StatusCode, payload);

        using JsonDocument document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("hourly", out JsonElement hourly)
            || !hourly.TryGetProperty("time", out JsonElement times)
            || times.ValueKind != JsonValueKind.Array)
            throw new ProviderAdapterException("open_meteo_response_invalid", "Open-Meteo returned no hourly time series.", true, (int)response.StatusCode);

        DateTimeOffset issuedAt = context.Now.ToUniversalTime();
        var records = new List<ProviderRecord>(times.GetArrayLength());
        for (int index = 0; index < times.GetArrayLength(); index++)
        {
            string? timeValue = times[index].GetString();
            if (!DateTimeOffset.TryParse(timeValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset observedAt)) continue;
            observedAt = observedAt.ToUniversalTime();
            decimal? temperature = Value(hourly, "temperature_2m", index);
            decimal? humidity = Value(hourly, "relative_humidity_2m", index);
            decimal? precipitation = Value(hourly, "precipitation", index);
            decimal? probability = Value(hourly, "precipitation_probability", index);
            decimal? wind = Value(hourly, "wind_speed_10m", index);
            decimal? direction = Value(hourly, "wind_direction_10m", index);
            decimal? pressure = Value(hourly, "surface_pressure", index);
            decimal? soilTemperature = Value(hourly, "soil_temperature_0cm", index);
            decimal? soilMoisture = Value(hourly, "soil_moisture_0_to_1cm", index) is { } fraction ? fraction * 100m : null;
            string externalId = $"{context.ExternalSourceId}:{observedAt:yyyyMMddHHmm}";
            string recordPayload = JsonSerializer.Serialize(new
            {
                time = observedAt,
                temperature_2m = temperature,
                relative_humidity_2m = humidity,
                precipitation,
                precipitation_probability = probability,
                wind_speed_10m = wind,
                wind_direction_10m = direction,
                surface_pressure = pressure,
                soil_temperature_0cm = soilTemperature,
                soil_moisture_0_to_1cm = soilMoisture is null ? null : soilMoisture / 100m
            });
            ProviderNormalizedRecord normalized = observedAt <= issuedAt
                ? new WeatherObservationValue(temperature, humidity, precipitation, wind, direction, pressure, soilTemperature, soilMoisture)
                : new WeatherForecastValue(issuedAt, observedAt.AddHours(1), temperature, temperature, precipitation, probability, wind, humidity);
            records.Add(new ProviderRecord(externalId, observedAt, null, recordPayload, normalized));
        }

        DateTimeOffset? latest = records.Count == 0 ? null : records.Max(x => x.ObservedAt);
        return new ProviderIngestionResult(records, JsonSerializer.Serialize(new { latestObservedAt = latest }), (int)response.StatusCode);
    }

    private static decimal? Value(JsonElement hourly, string name, int index)
    {
        if (!hourly.TryGetProperty(name, out JsonElement values) || values.ValueKind != JsonValueKind.Array || index >= values.GetArrayLength()) return null;
        JsonElement value = values[index];
        return value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out decimal result) ? result : null;
    }

    private static ProviderAdapterException Failure(string code, HttpStatusCode status, string payload)
    {
        bool retriable = (int)status is 408 or 425 or 429 || (int)status >= 500;
        string message = payload.Length > 1_000 ? payload[..1_000] : payload;
        return new ProviderAdapterException(code, $"Open-Meteo returned {(int)status}: {message}", retriable, (int)status);
    }
}

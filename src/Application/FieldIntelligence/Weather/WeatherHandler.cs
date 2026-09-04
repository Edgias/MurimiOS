using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Weather;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;

public sealed class WeatherHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<WeatherQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(WeatherQuery message, CancellationToken cancellationToken) => WeatherAsync(message.fieldId, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> WeatherAsync(Guid fieldId, DateTimeOffset? from, DateTimeOffset? to,
        int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FieldAllowedAsync(fieldId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        int take = Math.Clamp(limit ?? 250, 1, 2_000); DateTimeOffset start = from?.ToUniversalTime() ?? DateTimeOffset.UtcNow.AddDays(-7); DateTimeOffset end = to?.ToUniversalTime() ?? DateTimeOffset.UtcNow.AddDays(7);
        WeatherObservation[] observationRows = await db.WeatherObservations.AsNoTracking().Where(x => x.FieldId == fieldId && x.ObservedAt >= start && x.ObservedAt <= end)
            .OrderByDescending(x => x.ObservedAt).Take(take).ToArrayAsync(cancellationToken);
        WeatherForecast[] forecastRows = await db.WeatherForecasts.AsNoTracking().Where(x => x.FieldId == fieldId && x.ValidTo >= start && x.ValidFrom <= end)
            .OrderBy(x => x.ValidFrom).Take(take).ToArrayAsync(cancellationToken);
        WeatherObservationResponse[] observations = observationRows.Select(ToWeather).ToArray();
        WeatherForecastResponse[] forecasts = forecastRows.Select(ToForecast).ToArray();
        return UseCaseResults.Ok(new FieldWeatherResponse(observations, forecasts));
    }


    private static async Task<bool> FieldAllowedAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.AsNoTracking().Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        return farmId is { } farm && await scope.CanAccessFarmAsync(farm, cancellationToken);
    }


    private static WeatherObservationResponse ToWeather(WeatherObservation x) => new(x.Id, x.LocationId, x.FarmId,
        x.FieldId, x.FeedId, x.IngestionRecordId, x.ProviderObservationId, x.ObservedAt, x.ReceivedAt,
        x.TemperatureC, x.RelativeHumidityPercent, x.PrecipitationMm, x.WindSpeedMps,
        x.WindDirectionDegrees, x.PressureHpa, x.SoilTemperatureC, x.SoilMoisturePercent, x.Quality);

    private static WeatherForecastResponse ToForecast(WeatherForecast x) => new(x.Id, x.LocationId, x.FarmId,
        x.FieldId, x.FeedId, x.IngestionRecordId, x.ProviderForecastId, x.IssuedAt, x.ValidFrom, x.ValidTo,
        x.MinimumTemperatureC, x.MaximumTemperatureC, x.PrecipitationMm, x.PrecipitationProbabilityPercent,
        x.WindSpeedMps, x.RelativeHumidityPercent, x.Quality);

}

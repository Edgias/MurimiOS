using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IngestWeatherForecastsEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/ingestion/records/{recordId:guid}/weather-forecasts", async Task<IResult> (Guid recordId, IngestWeatherForecastsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherForecasts.IngestWeatherForecastsCommand(recordId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherForecasts.IngestWeatherForecastsRequest(request.FieldId, request.GridKey, request.Latitude, request.Longitude, request.ElevationMetres, request.Forecasts.Select(item => new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherForecasts.WeatherForecastItem(item.Id, item.ProviderForecastId, item.IssuedAt, item.ValidFrom, item.ValidTo, item.MinimumTemperatureC, item.MaximumTemperatureC, item.PrecipitationMm, item.PrecipitationProbabilityPercent, item.WindSpeedMps, item.RelativeHumidityPercent, item.Quality)).ToArray())), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.IntegrationsOperate, PlatformPermissions.FieldIntelligenceWrite);

    }
}

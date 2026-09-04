using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IngestWeatherObservationsEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {

        insights.MapPost("/ingestion/records/{recordId:guid}/weather-observations", async Task<IResult> (Guid recordId, IngestWeatherObservationsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherObservations.IngestWeatherObservationsCommand(recordId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherObservations.IngestWeatherObservationsRequest(request.FieldId, request.GridKey, request.Latitude, request.Longitude, request.ElevationMetres, request.Observations.Select(item => new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherObservations.WeatherObservationItem(item.Id, item.ProviderObservationId, item.ObservedAt, item.TemperatureC, item.RelativeHumidityPercent, item.PrecipitationMm, item.WindSpeedMps, item.WindDirectionDegrees, item.PressureHpa, item.SoilTemperatureC, item.SoilMoisturePercent, item.Quality)).ToArray())), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.IntegrationsOperate, PlatformPermissions.FieldIntelligenceWrite);

    }
}

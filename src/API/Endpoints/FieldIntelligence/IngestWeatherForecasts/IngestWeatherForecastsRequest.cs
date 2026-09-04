using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts;
public sealed record IngestWeatherForecastsRequest(Guid? FieldId, string GridKey, decimal Latitude,
    decimal Longitude, decimal? ElevationMetres, IReadOnlyList<WeatherForecastItem> Forecasts);

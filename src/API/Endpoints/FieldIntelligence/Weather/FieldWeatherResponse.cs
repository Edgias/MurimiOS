using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Weather;
using WeatherForecastResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared.WeatherForecastResponse;
using WeatherObservationResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared.WeatherObservationResponse;
public sealed record FieldWeatherResponse(IReadOnlyList<WeatherObservationResponse> Observations,
    IReadOnlyList<WeatherForecastResponse> Forecasts);

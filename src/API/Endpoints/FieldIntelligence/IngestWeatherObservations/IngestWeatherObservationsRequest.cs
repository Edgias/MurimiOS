using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations;

public sealed record IngestWeatherObservationsRequest(Guid? FieldId, string GridKey, decimal Latitude,
    decimal Longitude, decimal? ElevationMetres, IReadOnlyList<WeatherObservationItem> Observations);

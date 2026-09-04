using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Overview;
public sealed record FarmIntelligenceOverviewResponse(Guid FarmId, DateTimeOffset AsOf,
    DateTimeOffset? LatestWeatherAt, DateTimeOffset? LatestSatelliteAt, DateTimeOffset? LatestScoutingAt,
    int OpenAlerts, IReadOnlyList<InsightFeedFreshnessResponse> Feeds);

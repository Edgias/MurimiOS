using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite;
public sealed record IngestSatelliteStatisticsRequest(Guid FieldId, Guid BoundaryVersionId,
    IReadOnlyList<SatelliteStatisticItem> Statistics);

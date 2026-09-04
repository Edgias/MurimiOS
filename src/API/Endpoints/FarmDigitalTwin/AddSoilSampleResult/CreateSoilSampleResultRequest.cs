using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilSampleResult;
public sealed record CreateSoilSampleResultRequest(Guid? Id, string AnalyteCode, string AnalyteName,
    decimal Value, string Unit, string? Method, decimal? DetectionLimit, string? ResultFlag, DateTimeOffset? RecordedAt);

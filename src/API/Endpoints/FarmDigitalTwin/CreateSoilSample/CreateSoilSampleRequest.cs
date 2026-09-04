using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilSample;
public sealed record CreateSoilSampleRequest(Guid? Id, Guid FieldId, Guid FieldBoundaryVersionId,
    Guid? SoilZoneBoundaryVersionId, string SampleCode, double Longitude, double Latitude,
    DateTimeOffset SampledAt, string? SampledBy, decimal DepthFromCm, decimal DepthToCm,
    string? LaboratoryName, string? LaboratoryReference, string? Notes);

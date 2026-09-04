using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply;

public sealed record ApplyGeospatialImportRequest(IReadOnlyList<ApplyGeospatialImportFeature> Features);

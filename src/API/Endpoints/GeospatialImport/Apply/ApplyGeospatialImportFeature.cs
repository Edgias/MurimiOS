using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply;
public sealed record ApplyGeospatialImportFeature(Guid FeatureId, DateOnly ValidFrom, Guid? FieldId = null, string? FieldName = null);

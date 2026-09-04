using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.GeospatialImport.GetJob;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;
public sealed record GeospatialImportJobResponse(Guid Id, Guid FarmId, GeospatialImportFormat Format, string FileName,
    string ContentType, long ByteSize, string Sha256, string ParserVersion, GeospatialImportStatus Status,
    int FeatureCount, int EligibleFeatureCount, int WarningCount, int ErrorCount, string? FailureCode,
    string? FailureDetail, DateTimeOffset? ValidatedAt, DateTimeOffset? AppliedAt, string? AppliedBy,
    DateTimeOffset CreatedAt);

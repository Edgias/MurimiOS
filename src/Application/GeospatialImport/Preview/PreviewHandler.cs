using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.GeospatialImport.Preview;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;

public sealed class PreviewHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<PreviewQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PreviewQuery message, CancellationToken cancellationToken) => PreviewAsync(message.jobId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> PreviewAsync(Guid jobId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        GeospatialImportJob? job = await db.GeospatialImportJobs.AsNoTracking()
            .SingleOrDefaultAsync(value => value.Id == jobId, cancellationToken);
        if (job is null || !await scope.CanAccessFarmAsync(job.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        GeospatialImportFeature[] features = await db.GeospatialImportFeatures.AsNoTracking()
            .Where(feature => feature.JobId == jobId).OrderBy(feature => feature.Ordinal).ToArrayAsync(cancellationToken);
        return UseCaseResults.Json(new
        {
            type = "FeatureCollection",
            importJob = ToResponse(job),
            features = features.Select(feature => new
            {
                type = "Feature",
                id = feature.Id,
                properties = new
                {
                    feature.Ordinal,
                    feature.SourceName,
                    feature.AreaHa,
                    feature.IsEligible,
                    source = JsonDocument.Parse(feature.PropertiesJson).RootElement,
                    issues = JsonDocument.Parse(feature.IssuesJson).RootElement,
                    feature.AppliedFieldId,
                    feature.AppliedBoundaryVersionId
                },
                geometry = new { type = "MultiPolygon", coordinates = Coordinates(feature.Boundary) }
            })
        }, JsonOptions);
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static GeospatialImportJobResponse ToResponse(GeospatialImportJob job) => new(job.Id, job.FarmId,
        job.Format, job.FileName, job.ContentType, job.ByteSize, job.Sha256, job.ParserVersion, job.Status,
        job.FeatureCount, job.EligibleFeatureCount, job.WarningCount, job.ErrorCount, job.FailureCode,
        job.FailureDetail, job.ValidatedAt, job.AppliedAt, job.AppliedBy, job.CreatedAtUtc);


    private static object Coordinates(MultiPolygon geometry) => geometry.Geometries.Cast<Polygon>().Select(polygon =>
        new[] { polygon.ExteriorRing }.Concat(Enumerable.Range(0, polygon.NumInteriorRings).Select(polygon.GetInteriorRingN))
            .Select(ring => ring.Coordinates.Select(coordinate => new[] { coordinate.X, coordinate.Y }).ToArray()).ToArray()).ToArray();

}

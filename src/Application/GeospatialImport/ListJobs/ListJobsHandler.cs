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
namespace EdgIQ.MurimiOS.Application.GeospatialImport.ListJobs;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;

public sealed class ListJobsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListJobsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListJobsQuery message, CancellationToken cancellationToken) => ListJobsAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListJobsAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        GeospatialImportJobResponse[] jobs = await db.GeospatialImportJobs.AsNoTracking()
            .Where(job => job.FarmId == farmId).OrderByDescending(job => job.CreatedAtUtc)
            .Select(job => ToResponse(job)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(jobs);
    }


    private static GeospatialImportJobResponse ToResponse(GeospatialImportJob job) => new(job.Id, job.FarmId,
        job.Format, job.FileName, job.ContentType, job.ByteSize, job.Sha256, job.ParserVersion, job.Status,
        job.FeatureCount, job.EligibleFeatureCount, job.WarningCount, job.ErrorCount, job.FailureCode,
        job.FailureDetail, job.ValidatedAt, job.AppliedAt, job.AppliedBy, job.CreatedAtUtc);

}

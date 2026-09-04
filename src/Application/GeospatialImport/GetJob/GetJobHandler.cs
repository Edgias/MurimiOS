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
namespace EdgIQ.MurimiOS.Application.GeospatialImport.GetJob;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;

public sealed class GetJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetJobQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetJobQuery message, CancellationToken cancellationToken) => GetJobAsync(message.jobId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetJobAsync(Guid jobId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        GeospatialImportJob? job = await db.GeospatialImportJobs.AsNoTracking()
            .SingleOrDefaultAsync(value => value.Id == jobId, cancellationToken);
        if (job is null || !await scope.CanAccessFarmAsync(job.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(ToResponse(job));
    }


    private static GeospatialImportJobResponse ToResponse(GeospatialImportJob job) => new(job.Id, job.FarmId,
        job.Format, job.FileName, job.ContentType, job.ByteSize, job.Sha256, job.ParserVersion, job.Status,
        job.FeatureCount, job.EligibleFeatureCount, job.WarningCount, job.ErrorCount, job.FailureCode,
        job.FailureDetail, job.ValidatedAt, job.AppliedAt, job.AppliedBy, job.CreatedAtUtc);

}

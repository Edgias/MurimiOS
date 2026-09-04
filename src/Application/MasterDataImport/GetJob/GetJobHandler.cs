using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.GetJob;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;

public sealed class GetJobHandler(IPlatformDbContext db) : IRequestHandler<GetJobQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetJobQuery message, CancellationToken cancellationToken) => GetJobAsync(message.jobId, db, cancellationToken);

    private static async Task<UseCaseResult> GetJobAsync(Guid jobId, IPlatformDbContext db, CancellationToken ct)
    {
        MasterDataImportJob? job = await db.MasterDataImportJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == jobId, ct);
        return job is null ? UseCaseResults.NotFound() : UseCaseResults.Ok(Response(job));
    }

    private static object Response(MasterDataImportJob x) => new
    {
        x.Id, x.Type, x.FileName, x.ByteSize, x.Sha256, x.ParserVersion, x.Status, x.RowCount,
        x.ReadyRowCount, x.ErrorRowCount, x.AppliedRowCount, x.FailedRowCount, x.CheckpointOrdinal,
        x.ValidatedAt, x.ApplyStartedAt, x.AppliedAt, x.AppliedBy, x.FailureCode, x.FailureDetail, x.CreatedAtUtc
    };

}

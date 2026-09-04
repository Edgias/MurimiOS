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
namespace EdgIQ.MurimiOS.Application.MasterDataImport.ListJobs;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;

public sealed class ListJobsHandler(IPlatformDbContext db) : IRequestHandler<ListJobsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListJobsQuery message, CancellationToken cancellationToken) => ListJobsAsync(message.type, message.limit, db, cancellationToken);

    private static async Task<UseCaseResult> ListJobsAsync(MasterDataImportType? type, int? limit,
        IPlatformDbContext db, CancellationToken ct)
    {
        int take = Math.Clamp(limit ?? 100, 1, 500);
        IQueryable<MasterDataImportJob> query = db.MasterDataImportJobs.AsNoTracking();
        if (type.HasValue) query = query.Where(x => x.Type == type);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.CreatedAtUtc).Take(take)
            .Select(x => Response(x)).ToArrayAsync(ct));
    }

    private static object Response(MasterDataImportJob x) => new
    {
        x.Id, x.Type, x.FileName, x.ByteSize, x.Sha256, x.ParserVersion, x.Status, x.RowCount,
        x.ReadyRowCount, x.ErrorRowCount, x.AppliedRowCount, x.FailedRowCount, x.CheckpointOrdinal,
        x.ValidatedAt, x.ApplyStartedAt, x.AppliedAt, x.AppliedBy, x.FailureCode, x.FailureDetail, x.CreatedAtUtc
    };

}

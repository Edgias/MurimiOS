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
namespace EdgIQ.MurimiOS.Application.MasterDataImport.Preview;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;

public sealed class PreviewHandler(IPlatformDbContext db) : IRequestHandler<PreviewQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PreviewQuery message, CancellationToken cancellationToken) => PreviewAsync(message.jobId, message.offset, message.limit, db, cancellationToken);

    private static async Task<UseCaseResult> PreviewAsync(Guid jobId, int? offset, int? limit,
        IPlatformDbContext db, CancellationToken ct)
    {
        MasterDataImportJob? job = await db.MasterDataImportJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == jobId, ct);
        if (job is null) return UseCaseResults.NotFound();
        int skip = Math.Max(0, offset ?? 0);
        int take = Math.Clamp(limit ?? 200, 1, 1_000);
        MasterDataImportRow[] rows = await db.MasterDataImportRows.AsNoTracking().Where(x => x.JobId == jobId)
            .OrderBy(x => x.Ordinal).Skip(skip).Take(take).ToArrayAsync(ct);
        return UseCaseResults.Ok(new
        {
            job = Response(job), offset = skip, limit = take,
            rows = rows.Select(x => new
            {
                x.Id, x.Ordinal, x.Status, raw = Element(x.RawJson), normalized = Element(x.NormalizedJson),
                issues = Element(x.IssuesJson), x.AppliedEntityType, x.AppliedEntityId, x.AppliedFarmId,
                x.AppliedAt, x.FailureCode, x.FailureDetail
            })
        });
    }

    private static JsonElement Element(string json) { using JsonDocument document = JsonDocument.Parse(json); return document.RootElement.Clone(); }

    private static object Response(MasterDataImportJob x) => new
    {
        x.Id, x.Type, x.FileName, x.ByteSize, x.Sha256, x.ParserVersion, x.Status, x.RowCount,
        x.ReadyRowCount, x.ErrorRowCount, x.AppliedRowCount, x.FailedRowCount, x.CheckpointOrdinal,
        x.ValidatedAt, x.ApplyStartedAt, x.AppliedAt, x.AppliedBy, x.FailureCode, x.FailureDetail, x.CreatedAtUtc
    };

}

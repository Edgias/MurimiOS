using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Application.Common.Imports;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.Errors;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
using MasterDataImportRowStatus = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRowStatus;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;

public sealed class ErrorsHandler(IPlatformDbContext db) : IRequestHandler<ErrorsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ErrorsQuery message, CancellationToken cancellationToken) => ErrorsAsync(message.jobId, db, cancellationToken);

    private static async Task<UseCaseResult> ErrorsAsync(Guid jobId, IPlatformDbContext db, CancellationToken ct)
    {
        MasterDataImportJob? job = await db.MasterDataImportJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == jobId, ct);
        if (job is null) return UseCaseResults.NotFound();
        MasterDataImportRow[] rows = await db.MasterDataImportRows.AsNoTracking()
            .Where(x => x.JobId == jobId && (x.Status == MasterDataImportRowStatus.Invalid || x.Status == MasterDataImportRowStatus.Failed))
            .OrderBy(x => x.Ordinal).ToArrayAsync(ct);
        var csv = new List<IEnumerable<string?>> { new[] { "ordinal", "status", "code", "column", "message", "raw_json" } };
        foreach (MasterDataImportRow row in rows)
        {
            MasterDataCsvIssue[] issues = row.Status == MasterDataImportRowStatus.Failed
                ? [new(row.FailureCode ?? "apply_failed", null, row.FailureDetail ?? "Row application failed.")]
                : JsonSerializer.Deserialize<MasterDataCsvIssue[]>(row.IssuesJson, JsonOptions) ?? [];
            foreach (MasterDataCsvIssue issue in issues)
                csv.Add([row.Ordinal.ToString(CultureInfo.InvariantCulture), row.Status.ToString(), issue.Code,
                    issue.Column, issue.Message, row.RawJson]);
        }
        return UseCaseResults.File(Encoding.UTF8.GetBytes(MasterDataCsv.Write(csv)), "text/csv; charset=utf-8",
            $"murimios-{Slug(job.Type)}-{job.Id:N}-errors.csv");
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static string Slug(MasterDataImportType type) => type.ToString().ToLowerInvariant();

}

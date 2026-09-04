using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.List;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;

public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(message.farmId, message.assessmentId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListAsync(Guid farmId, Guid assessmentId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        bool assessmentExists = await db.ComplianceAssessments.AsNoTracking()
            .AnyAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
        if (!assessmentExists) return UseCaseResults.NotFound();
        ComplianceAuditPackExport[] exports = await db.ComplianceAuditPackExports.AsNoTracking()
            .Where(x => x.AssessmentId == assessmentId && x.FarmId == farmId)
            .OrderByDescending(x => x.Revision).ToArrayAsync(ct);
        return UseCaseResults.Ok(exports.Select(Metadata));
    }


    private static object Metadata(ComplianceAuditPackExport export) => new
    {
        export.Id, export.AssessmentId, export.FarmId, export.StandardVersionId, export.Revision,
        export.ContentSchemaVersion, export.ScopeStatement, export.IncludesEvidenceFiles, export.Format,
        export.ByteSize, export.ContentEntryCount, export.EvidenceFileCount, export.ArtifactSha256,
        export.ManifestSha256, export.SignatureAlgorithm, export.SignatureKeyId, export.SignatureBase64,
        export.GeneratedAt, export.GeneratedBy
    };

}

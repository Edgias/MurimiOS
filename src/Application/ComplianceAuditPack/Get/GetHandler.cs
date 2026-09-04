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
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Get;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;

public sealed class GetHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetQuery message, CancellationToken cancellationToken) => GetAsync(message.farmId, message.exportId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetAsync(Guid farmId, Guid exportId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAuditPackExport? export = await db.ComplianceAuditPackExports.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == exportId && x.FarmId == farmId, ct);
        return export is null ? UseCaseResults.NotFound() : UseCaseResults.Ok(Metadata(export));
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

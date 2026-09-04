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
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Download;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;

public sealed class DownloadHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IMediaStorage storage, UseCaseContext context) : IRequestHandler<DownloadQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DownloadQuery message, CancellationToken cancellationToken) => DownloadAsync(message.farmId, message.exportId, db, scope, storage, context, cancellationToken);

    private static async Task<UseCaseResult> DownloadAsync(Guid farmId, Guid exportId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IMediaStorage storage, UseCaseContext context, CancellationToken ct)
    {
        ComplianceAuditPackExport? export = await FindAsync(farmId, exportId, db, scope, ct);
        if (export is null) return UseCaseResults.NotFound();
        Stream stream;
        try { stream = await storage.OpenReadAsync(export.ObjectKey, ct); }
        catch (Exception exception) when (exception is FileNotFoundException or DirectoryNotFoundException)
        { return Conflict("audit_pack_artifact_missing", "The retained audit-pack artifact is unavailable."); }
        IntegrityHeaders(context, export);
        context.Response["Cache-Control"] = "no-store";
        return UseCaseResults.Stream(stream, "application/zip",
            $"compliance-audit-pack-{export.AssessmentId:N}-r{export.Revision}.zip", enableRangeProcessing: false);
    }


    private static async Task<ComplianceAuditPackExport?> FindAsync(Guid farmId, Guid exportId,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return null;
        return await db.ComplianceAuditPackExports.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == exportId && x.FarmId == farmId, ct);
    }


    private static void IntegrityHeaders(UseCaseContext context, ComplianceAuditPackExport export)
    {
        context.Response["X-Artifact-SHA256"] = export.ArtifactSha256;
        context.Response["X-Manifest-SHA256"] = export.ManifestSha256;
        context.Response["X-Signature-Algorithm"] = export.SignatureAlgorithm;
        context.Response["X-Signature-Key-Id"] = export.SignatureKeyId;
        context.Response["X-Content-Type-Options"] = "nosniff";
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

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
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Manifest;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;

public sealed class ManifestHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, UseCaseContext context) : IRequestHandler<ManifestQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ManifestQuery message, CancellationToken cancellationToken) => ManifestAsync(message.farmId, message.exportId, db, scope, context, cancellationToken);

    private static async Task<UseCaseResult> ManifestAsync(Guid farmId, Guid exportId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, UseCaseContext context, CancellationToken ct)
    {
        ComplianceAuditPackExport? export = await FindAsync(farmId, exportId, db, scope, ct);
        if (export is null) return UseCaseResults.NotFound();
        IntegrityHeaders(context, export);
        return UseCaseResults.Text(export.ManifestJson, "application/json");
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

}

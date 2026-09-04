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
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Verify;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;

public sealed class VerifyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IMediaStorage storage, IComplianceAuditPackSignatureService signer, IOptions<ComplianceAuditPackOptions> configured) : IRequestHandler<VerifyQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(VerifyQuery message, CancellationToken cancellationToken) => VerifyAsync(message.farmId, message.exportId, db, scope, storage, signer, configured, cancellationToken);

    private static async Task<UseCaseResult> VerifyAsync(Guid farmId, Guid exportId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IMediaStorage storage, IComplianceAuditPackSignatureService signer,
        IOptions<ComplianceAuditPackOptions> configured, CancellationToken ct)
    {
        ComplianceAuditPackExport? export = await FindAsync(farmId, exportId, db, scope, ct);
        if (export is null) return UseCaseResults.NotFound();
        try
        {
            await using Stream source = await storage.OpenReadAsync(export.ObjectKey, ct);
            if (source.CanSeek && source.Length > configured.Value.MaximumArtifactBytes)
                return Conflict("audit_pack_artifact_oversize", "The retained artifact exceeds its configured verification limit.");
            await using var copy = new MemoryStream();
            await source.CopyToAsync(copy, ct);
            byte[] artifact = copy.ToArray();
            bool artifactHashValid = Sha(artifact).Equals(export.ArtifactSha256, StringComparison.Ordinal);
            using var zip = new ZipArchive(new MemoryStream(artifact, writable: false), ZipArchiveMode.Read);
            ZipArchiveEntry? manifestEntry = zip.GetEntry("manifest.json");
            ZipArchiveEntry? signatureEntry = zip.GetEntry("signature.json");
            if (manifestEntry is null || signatureEntry is null)
                return Conflict("audit_pack_structure_invalid", "The retained artifact has no manifest or signature entry.");
            byte[] manifest = await ReadAsync(manifestEntry, configured.Value.MaximumArtifactBytes, ct);
            byte[] signatureDocument = await ReadAsync(signatureEntry, 16_384, ct);
            bool manifestHashValid = Sha(manifest).Equals(export.ManifestSha256, StringComparison.Ordinal);
            bool signatureValid = signer.Verify(export.SignatureKeyId, manifest, export.SignatureBase64);
            using JsonDocument signature = JsonDocument.Parse(signatureDocument);
            JsonElement root = signature.RootElement;
            bool signatureDocumentValid = root.TryGetProperty("algorithm", out JsonElement algorithm)
                && algorithm.GetString() == export.SignatureAlgorithm
                && root.TryGetProperty("keyId", out JsonElement keyId) && keyId.GetString() == export.SignatureKeyId
                && root.TryGetProperty("manifestSha256", out JsonElement hash) && hash.GetString() == export.ManifestSha256
                && root.TryGetProperty("signatureBase64", out JsonElement value) && value.GetString() == export.SignatureBase64;
            bool valid = artifactHashValid && manifestHashValid && signatureValid && signatureDocumentValid;
            return UseCaseResults.Ok(new
            {
                export.Id, export.Revision, valid, artifactHashValid, manifestHashValid, signatureValid,
                signatureDocumentValid, export.ArtifactSha256, export.ManifestSha256,
                export.SignatureAlgorithm, export.SignatureKeyId, verifiedAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException or JsonException)
        { return Conflict("audit_pack_verification_failed", exception.Message); }
    }


    private static async Task<ComplianceAuditPackExport?> FindAsync(Guid farmId, Guid exportId,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return null;
        return await db.ComplianceAuditPackExports.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == exportId && x.FarmId == farmId, ct);
    }


    private static async Task<byte[]> ReadAsync(ZipArchiveEntry entry, long maximum, CancellationToken ct)
    {
        if (entry.Length > maximum) throw new InvalidDataException($"ZIP entry {entry.FullName} is too large.");
        await using Stream stream = entry.Open();
        await using var result = new MemoryStream();
        await stream.CopyToAsync(result, ct);
        if (result.Length > maximum) throw new InvalidDataException($"ZIP entry {entry.FullName} is too large.");
        return result.ToArray();
    }


    private static string Sha(byte[] content) => Convert.ToHexStringLower(SHA256.HashData(content));

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

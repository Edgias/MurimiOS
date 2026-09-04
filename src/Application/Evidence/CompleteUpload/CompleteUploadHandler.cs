using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.Observability;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.CompleteUpload;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CompleteUploadHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IMediaStorage storage, IMediaMalwareScanner malwareScanner, ITenantContext tenantContext, TimeProvider timeProvider) : IRequestHandler<CompleteUploadCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CompleteUploadCommand message, CancellationToken cancellationToken) => CompleteUploadAsync(message.uploadId, db, scopeAccess, storage, malwareScanner, tenantContext, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> CompleteUploadAsync(
        Guid uploadId,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        IMediaStorage storage,
        IMediaMalwareScanner malwareScanner,
        ITenantContext tenantContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        MediaUploadSession? upload = await db.MediaUploadSessions.SingleOrDefaultAsync(x => x.Id == uploadId, cancellationToken);
        if (upload is null)
        {
            return UseCaseResults.NotFound();
        }
        Guid? farmId = await FarmIdForCropZoneAsync(upload.CropZoneId, db, cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            return UseCaseResults.NotFound();

        EvidenceAttachment? existing = await db.EvidenceAttachments
            .SingleOrDefaultAsync(x => x.Id == upload.EvidenceId, cancellationToken);
        if (existing is not null)
        {
            return UseCaseResults.Ok(ToResponse(existing));
        }

        if (upload.ReceivedBytes != upload.ExpectedBytes)
        {
            return UseCaseResults.Conflict(new
            {
                code = "upload_incomplete",
                expectedOffset = upload.ReceivedBytes,
                expectedBytes = upload.ExpectedBytes
            });
        }

        MediaInspection inspection = await storage.InspectPendingAsync(upload.TenantId, upload.Id, cancellationToken);
        if (inspection.Length != upload.ExpectedBytes
            || !inspection.Sha256.Equals(upload.ExpectedSha256, StringComparison.OrdinalIgnoreCase))
        {
            return UseCaseResults.UnprocessableEntity(new
            {
                code = "media_integrity_failed",
                message = "Uploaded bytes do not match the declared size and SHA-256 digest."
            });
        }

        if (!MediaSignatureMatches(upload.ContentType, inspection.Header))
        {
            return UseCaseResults.UnprocessableEntity(new
            {
                code = "media_signature_invalid",
                message = "The uploaded bytes do not match the declared image type."
            });
        }

        await using (Stream pending = await storage.OpenPendingReadAsync(upload.TenantId, upload.Id, cancellationToken))
        {
            MediaScanResult scan = await malwareScanner.ScanAsync(pending, cancellationToken);
            if (scan.Verdict == MediaScanVerdict.ThreatDetected)
            {
                PlatformDiagnostics.MediaScanFailures.Add(1, new KeyValuePair<string, object?>("verdict", "threat"));
                return UseCaseResults.UnprocessableEntity(new
                {
                    code = "media_threat_detected",
                    message = "The uploaded file failed malware scanning."
                });
            }
            if (scan.Verdict == MediaScanVerdict.Unavailable)
            {
                PlatformDiagnostics.MediaScanFailures.Add(1, new KeyValuePair<string, object?>("verdict", "unavailable"));
                return UseCaseResults.Problem(
                    statusCode: 503,
                    title: "Media scanner unavailable",
                    extensions: new Dictionary<string, object?> { ["code"] = "media_scanner_unavailable" });
            }
        }

        string objectKey = await storage.CommitAsync(
            upload.TenantId,
            upload.Id,
            upload.EvidenceId,
            upload.ContentType,
            cancellationToken);
        PlatformDiagnostics.MediaCommitted.Add(
            1,
            new KeyValuePair<string, object?>("content.type", upload.ContentType));
        DateTimeOffset completedAt = timeProvider.GetUtcNow();
        var evidence = new EvidenceAttachment(
            upload.EvidenceId,
            upload.CropZoneId,
            upload.FileName,
            upload.ContentType,
            upload.ExpectedBytes,
            upload.ExpectedSha256,
            objectKey,
            upload.CapturedAt,
            upload.Latitude,
            upload.Longitude,
            upload.AccuracyMetres);
        upload.Complete(completedAt);
        db.EvidenceAttachments.Add(evidence);

        string payload = JsonSerializer.Serialize(new
        {
            evidence.Id,
            evidence.CropZoneId,
            evidence.FileName,
            evidence.ContentType,
            evidence.ByteSize,
            evidence.Sha256,
            evidence.CapturedAt,
            evidence.Latitude,
            evidence.Longitude,
            evidence.AccuracyMetres
        });
        db.SyncChanges.Add(new SyncChange(
            tenantContext.TenantId,
            farmId.Value,
            "EvidenceAttachment",
            evidence.Id,
            "Upsert",
            payload,
            completedAt));
        db.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(),
            tenantContext.TenantId,
            "Evidence.Captured",
            payload,
            completedAt));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToResponse(evidence));
    }


    private static EvidenceResponse ToResponse(EvidenceAttachment evidence) => new(
        evidence.Id,
        evidence.CropZoneId,
        evidence.FileName,
        evidence.ContentType,
        evidence.ByteSize,
        evidence.Sha256,
        evidence.CapturedAt,
        evidence.Latitude,
        evidence.Longitude,
        evidence.AccuracyMetres,
        $"/api/v1/evidence/{evidence.Id}/content");


    private static Task<Guid?> FarmIdForCropZoneAsync(
        Guid cropZoneId,
        IPlatformDbContext db,
        CancellationToken cancellationToken) =>
        (from zone in db.CropZones
         join season in db.GrowingSeasons on zone.SeasonId equals season.Id
         where zone.Id == cropZoneId
         select (Guid?)season.FarmId).SingleOrDefaultAsync(cancellationToken);


    private static UseCaseResult Problem(string code, string detail) => UseCaseResults.Problem(
        statusCode: 400,
        title: "Evidence request rejected",
        detail: detail,
        extensions: new Dictionary<string, object?> { ["code"] = code });


    private static bool MediaSignatureMatches(string contentType, byte[] header) => contentType switch
    {
        "image/jpeg" => header.Length >= 3 && header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff,
        "image/png" => header.Length >= 8
            && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }),
        "image/webp" => header.Length >= 12
            && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
            && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
        _ => false
    };

}

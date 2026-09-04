using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.InitiateUpload;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using MediaUploadStatus = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadStatus;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;

public sealed class InitiateUploadHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, TimeProvider timeProvider) : IRequestHandler<InitiateUploadCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(InitiateUploadCommand message, CancellationToken cancellationToken) => InitiateUploadAsync(message.request, db, scopeAccess, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> InitiateUploadAsync(
        InitiateEvidenceUploadRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (request.EvidenceId == Guid.Empty || request.CropZoneId == Guid.Empty)
        {
            return Problem("evidence_context_required", "Evidence and crop-zone identifiers are required.");
        }

        Guid? farmId = await FarmIdForCropZoneAsync(request.CropZoneId, db, cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
        {
            return Problem("crop_zone_not_found", "The crop zone is unavailable in this organisation scope.");
        }

        if (request.ByteSize is <= 0 or > MaximumUploadBytes)
        {
            return Problem("invalid_media_size", $"Evidence must be between 1 byte and {MaximumUploadBytes} bytes.");
        }

        string contentType = request.ContentType?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!AllowedContentTypes.Contains(contentType))
        {
            return Problem("unsupported_media_type", "Only JPEG, PNG, and WebP evidence is accepted.");
        }

        string fileName = SanitizeFileName(request.FileName);
        if (fileName.Length == 0)
        {
            return Problem("invalid_file_name", "A file name is required.");
        }

        EvidenceAttachment? completed = await db.EvidenceAttachments
            .SingleOrDefaultAsync(x => x.Id == request.EvidenceId, cancellationToken);
        if (completed is not null)
        {
            return UseCaseResults.Ok(new EvidenceUploadResponse(
                Guid.Empty,
                completed.Id,
                completed.ByteSize,
                ChunkSize,
                timeProvider.GetUtcNow(),
                true));
        }

        MediaUploadSession? existing = await db.MediaUploadSessions
            .SingleOrDefaultAsync(x => x.EvidenceId == request.EvidenceId, cancellationToken);
        if (existing is not null)
        {
            bool sameUpload = existing.CropZoneId == request.CropZoneId
                && existing.ExpectedBytes == request.ByteSize
                && existing.ExpectedSha256.Equals(request.Sha256, StringComparison.OrdinalIgnoreCase)
                && existing.ContentType.Equals(contentType, StringComparison.OrdinalIgnoreCase);
            if (!sameUpload)
            {
                return UseCaseResults.Conflict(new
                {
                    code = "evidence_id_reused",
                    message = "An evidence id cannot be reused with different media metadata."
                });
            }

            return UseCaseResults.Ok(new EvidenceUploadResponse(
                existing.Id,
                existing.EvidenceId,
                existing.ReceivedBytes,
                ChunkSize,
                existing.ExpiresAt,
                existing.Status == MediaUploadStatus.Completed));
        }

        DateTimeOffset now = timeProvider.GetUtcNow();
        MediaUploadSession upload;
        try
        {
            upload = new MediaUploadSession(
                Guid.NewGuid(),
                request.EvidenceId,
                request.CropZoneId,
                fileName,
                contentType,
                request.ByteSize,
                request.Sha256,
                request.CapturedAt,
                request.Latitude,
                request.Longitude,
                request.AccuracyMetres,
                now.AddDays(7));
        }
        catch (ArgumentException exception)
        {
            return Problem("invalid_evidence_metadata", exception.Message);
        }

        db.MediaUploadSessions.Add(upload);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(new EvidenceUploadResponse(
            upload.Id,
            upload.EvidenceId,
            upload.ReceivedBytes,
            ChunkSize,
            upload.ExpiresAt,
            false));
    }

    private const int ChunkSize = 1024 * 1024;

    private const long MaximumUploadBytes = 20 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };


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


    private static string SanitizeFileName(string? value)
    {
        string fileName = Path.GetFileName(value?.Trim() ?? string.Empty);
        return string.Concat(fileName.Where(character => !char.IsControl(character))).Trim();
    }

}

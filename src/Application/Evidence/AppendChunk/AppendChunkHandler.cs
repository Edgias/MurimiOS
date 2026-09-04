using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.AppendChunk;
using MediaUploadStatus = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadStatus;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;

public sealed class AppendChunkHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IMediaStorage storage, TimeProvider timeProvider) : IRequestHandler<AppendChunkCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AppendChunkCommand message, CancellationToken cancellationToken) => AppendChunkAsync(message.uploadId, message.request, db, scopeAccess, storage, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> AppendChunkAsync(
        Guid uploadId,
        IncomingContent request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        IMediaStorage storage,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (!long.TryParse(request.FirstHeader("Upload-Offset"), out long requestedOffset)
            || requestedOffset < 0)
        {
            return Problem("upload_offset_required", "Supply a non-negative Upload-Offset header.");
        }

        if (request.ContentLength is null or <= 0 or > ChunkSize)
        {
            return UseCaseResults.Problem(
                statusCode: 413,
                title: "Invalid media chunk",
                detail: $"Each chunk must contain between 1 and {ChunkSize} bytes.",
                extensions: new Dictionary<string, object?> { ["code"] = "invalid_chunk_size" });
        }

        MediaUploadSession? upload = await db.MediaUploadSessions.SingleOrDefaultAsync(x => x.Id == uploadId, cancellationToken);
        if (upload is null)
        {
            return UseCaseResults.NotFound();
        }
        Guid? farmId = await FarmIdForCropZoneAsync(upload.CropZoneId, db, cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            return UseCaseResults.NotFound();

        if (upload.Status == MediaUploadStatus.Completed)
        {
            return UseCaseResults.Ok(new EvidenceUploadOffsetResponse(upload.ReceivedBytes));
        }

        if (upload.ExpiresAt <= timeProvider.GetUtcNow())
        {
            return UseCaseResults.Problem(
                statusCode: 410,
                title: "Upload expired",
                extensions: new Dictionary<string, object?> { ["code"] = "upload_expired" });
        }

        if (requestedOffset != upload.ReceivedBytes)
        {
            return OffsetConflict(upload.ReceivedBytes);
        }

        if (upload.ReceivedBytes == upload.ExpectedBytes)
        {
            return UseCaseResults.Ok(new EvidenceUploadOffsetResponse(upload.ReceivedBytes));
        }

        long maximumBytes = Math.Min(ChunkSize, upload.ExpectedBytes - upload.ReceivedBytes);
        try
        {
            long storedLength = await storage.AppendChunkAsync(
                upload.TenantId,
                upload.Id,
                requestedOffset,
                request.Body,
                maximumBytes,
                cancellationToken);
            upload.Advance(requestedOffset, storedLength - requestedOffset);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Ok(new EvidenceUploadOffsetResponse(upload.ReceivedBytes));
        }
        catch (MediaOffsetMismatchException exception)
        {
            upload.ReconcileStoredBytes(exception.ExpectedOffset);
            await db.SaveChangesAsync(cancellationToken);
            return OffsetConflict(exception.ExpectedOffset);
        }
        catch (MediaChunkSizeException exception)
        {
            return UseCaseResults.Problem(
                statusCode: 413,
                title: "Media chunk is too large",
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = "chunk_too_large",
                    ["maximumChunkBytes"] = exception.MaximumChunkBytes
                });
        }
        catch (DbUpdateConcurrencyException)
        {
            return UseCaseResults.Conflict(new { code = "upload_concurrency", message = "Refresh the upload offset and retry." });
        }
    }

    private const int ChunkSize = 1024 * 1024;


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


    private static UseCaseResult OffsetConflict(long expectedOffset) => UseCaseResults.Conflict(new
    {
        code = "upload_offset_mismatch",
        expectedOffset
    });

}

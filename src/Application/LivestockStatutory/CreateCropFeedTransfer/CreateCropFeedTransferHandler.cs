using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.CreateCropFeedTransfer;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;
using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class CreateCropFeedTransferHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<CreateCropFeedTransferCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCropFeedTransferCommand message, CancellationToken cancellationToken) => CreateCropFeedTransferAsync(message.farmId, message.request, db, scope, actor, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateCropFeedTransferAsync(Guid farmId, CreateCropFeedTransferRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
        {
            string lockKey = $"crop-feed:{request.SourceType}:{request.SourceId:N}";
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
        }
        CropFeedSource? source = await LoadSourceAsync(farmId, request.SourceType, request.SourceId, db, cancellationToken);
        if (source is null) return Bad("crop_feed_source_not_found", "The crop lot was not found on this farm.");
        MeasuredQuantity measured;
        try { measured = MeasurementUnits.Capture(request.Quantity, request.Unit, request.Unit); }
        catch (ArgumentException ex) { return Bad("crop_feed_quantity_invalid", ex.Message); }
        if (!measured.CanonicalUnit.Equals(source.Unit, StringComparison.OrdinalIgnoreCase))
            return Bad("crop_feed_unit_mismatch", $"The source lot is measured in {source.Unit}.");
        decimal used = await db.CropFeedTransfers.Where(x => x.SourceType == request.SourceType && x.SourceId == request.SourceId)
            .SumAsync(x => (decimal?)x.Quantity, cancellationToken) ?? 0;
        if (used + measured.CanonicalValue > source.Quantity)
            return Conflict("crop_feed_source_insufficient", $"Only {source.Quantity - used} {source.Unit} remains available for feed conversion.");
        if (await db.LivestockFeedBatches.AnyAsync(x => x.FarmId == farmId && x.BatchCode == request.BatchCode.Trim(), cancellationToken))
            return Conflict("feed_batch_code_exists", "A feed batch with this code already exists on the farm.");

        Guid transferId = Id(request.Id);
        Guid feedBatchId = Id(request.FeedBatchId);
        Guid traceEventId = Guid.NewGuid();
        var batch = new LivestockFeedBatch(feedBatchId, farmId, request.BatchCode, request.FeedName,
            request.Quantity, request.Unit, DateOnly.FromDateTime(request.TransferredAt.UtcDateTime), request.ExpiresOn, "Home-grown crop transfer");
        var transfer = new CropFeedTransfer(transferId, farmId, request.SourceType, request.SourceId,
            feedBatchId, traceEventId, request.Quantity, request.Unit, request.TransferredAt, actor.Subject, request.Notes);
        TraceObjectType sourceTraceType = request.SourceType == CropFeedSourceType.HarvestLot
            ? TraceObjectType.HarvestLot : TraceObjectType.StorageLot;
        var traceEvent = new TraceEvent(traceEventId, farmId, TraceEventType.Transformation, "crop-to-feed",
            "feed-available", request.TransferredAt, "farm", farmId, nameof(CropFeedTransfer), transferId,
            request.Reference, JsonSerializer.Serialize(new { source.Crop, source.LotCode, feedBatchId, request.Notes }));
        db.LivestockFeedBatches.Add(batch);
        db.CropFeedTransfers.Add(transfer);
        db.TraceEvents.Add(traceEvent);
        db.TraceEventObjects.AddRange(
            new TraceEventObject(Guid.NewGuid(), traceEventId, sourceTraceType, request.SourceId, TraceObjectRole.Input, request.Quantity, request.Unit),
            new TraceEventObject(Guid.NewGuid(), traceEventId, TraceObjectType.LivestockFeedBatch, feedBatchId, TraceObjectRole.Output, request.Quantity, request.Unit));
        db.TraceLinks.Add(new TraceLink(Guid.NewGuid(), traceEventId, sourceTraceType, request.SourceId,
            TraceObjectType.LivestockFeedBatch, feedBatchId, TraceRelationshipType.ConvertedToFeed));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockFeed.CropTransferred",
            JsonSerializer.Serialize(new { transfer.Id, transfer.FarmId, transfer.SourceType, transfer.SourceId, transfer.FeedBatchId, transfer.Quantity, transfer.Unit }), request.TransferredAt));
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/farms/{farmId}/livestock/crop-feed-transfers/{transferId}", ToResponse(transfer));
    }


    private static async Task<CropFeedSource?> LoadSourceAsync(Guid farmId, CropFeedSourceType type, Guid sourceId,
        IPlatformDbContext db, CancellationToken cancellationToken) => type switch
    {
        CropFeedSourceType.HarvestLot => await (from lot in db.HarvestLots
            join zone in db.CropZones on lot.CropZoneId equals zone.Id
            join season in db.GrowingSeasons on zone.SeasonId equals season.Id
            where lot.Id == sourceId && season.FarmId == farmId
            select new CropFeedSource(lot.Quantity, lot.Unit, zone.Crop, lot.LotCode)).SingleOrDefaultAsync(cancellationToken),
        CropFeedSourceType.StorageLot => await db.StorageLots.Where(x => x.Id == sourceId && x.FarmId == farmId)
            .Select(x => new CropFeedSource(x.Quantity, x.Unit, x.Crop, x.LotCode)).SingleOrDefaultAsync(cancellationToken),
        _ => null
    };

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static StatutoryMovementDocumentResponse ToResponse(LivestockStatutoryMovementDocument x,
        IReadOnlyCollection<LivestockStatutorySubmissionAttempt> attempts) => new(x.Id, x.MovementId, x.SubjectId,
        x.FarmId, x.RegulatoryProfileId, x.Jurisdiction, x.ProviderCode, x.SchemaVersion, x.DocumentNumber,
        x.DocumentMediaType,
        JsonSerializer.Deserialize<JsonElement>(x.SnapshotJson), x.PayloadSha256, x.Status, x.CreatedAt, x.CreatedBy,
        x.ReportingDueAt, x.RegisterDueAt, x.SubmittedAt, x.DecidedAt, x.ExternalReference, x.RejectionCode,
        x.RejectionMessage, attempts.Select(a => new StatutorySubmissionAttemptResponse(a.Id, a.AttemptNumber,
            a.Outcome, a.StartedAt, a.CompletedAt, a.HttpStatus, a.ExternalReference, a.ErrorCode, a.ErrorMessage)).ToArray());

    private static CropFeedTransferResponse ToResponse(CropFeedTransfer x) => new(x.Id, x.FarmId, x.SourceType,
        x.SourceId, x.FeedBatchId, x.TraceEventId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit,
        x.TransferredAt, x.TransferredBy, x.Notes);

    private sealed record CropFeedSource(decimal Quantity, string Unit, string Crop, string LotCode);

}

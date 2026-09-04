using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.CorrectLivestockMeasurement;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CorrectLivestockMeasurementHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IRequestActorContext actor, ITenantContext tenant, TimeProvider timeProvider) : IRequestHandler<CorrectLivestockMeasurementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CorrectLivestockMeasurementCommand message, CancellationToken cancellationToken) => CorrectLivestockMeasurementAsync(message.measurementId, message.request, db, scopeAccess, actor, tenant, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> CorrectLivestockMeasurementAsync(
        Guid measurementId,
        CorrectLivestockMeasurementRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        IRequestActorContext actor,
        ITenantContext tenant,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (!ValidText(request.Metric, 80) || !ValidText(request.Unit, 30)
            || !ValidText(request.Reason, 2_000) || request.OccurredAt == default)
            return UseCaseResults.BadRequest(new { code = "invalid_correction", message = "Metric, unit, occurrence time, and a correction reason are required." });
        var context = await (
            from measurement in db.LivestockMeasurements
            join subject in db.LivestockSubjects on measurement.SubjectId equals subject.Id
            where measurement.Id == measurementId
            select new { Measurement = measurement, subject.FarmId })
            .SingleOrDefaultAsync(cancellationToken);
        if (context is null || !await scopeAccess.CanAccessFarmAsync(context.FarmId, cancellationToken))
            return UseCaseResults.NotFound();
        if (context.Measurement.SupersededById is not null)
            return UseCaseResults.Conflict(new { code = "already_superseded", replacementId = context.Measurement.SupersededById });

        Guid replacementId = NonEmptyOrNew(request.ReplacementId);
        Guid commandId = NonEmptyOrNew(request.CommandId);
        Guid correctionId = NonEmptyOrNew(request.CorrectionId);
        DateTimeOffset now = timeProvider.GetUtcNow();
        var replacement = new LivestockMeasurementRecord(
            replacementId,
            context.Measurement.SubjectId,
            request.Metric,
            request.Value,
            request.Unit,
            request.OccurredAt,
            now,
            commandId);
        context.Measurement.Supersede(replacementId, request.Reason);
        var correction = new CorrectionRecord(
            correctionId,
            typeof(LivestockMeasurementRecord).FullName!,
            context.Measurement.Id,
            replacement.Id,
            request.Reason,
            now,
            actor.Subject,
            context.FarmId);
        db.LivestockMeasurements.Add(replacement);
        db.CorrectionRecords.Add(correction);
        AddMessages(db, tenant.TenantId, context.FarmId, correction, new
        {
            correction.Id,
            OriginalMeasurementId = context.Measurement.Id,
            ReplacementMeasurementId = replacement.Id,
            replacement.SubjectId,
            replacement.Metric,
            replacement.Value,
            replacement.Unit,
            replacement.OccurredAt,
            replacement.RecordedAt,
            correction.Reason,
            correction.CorrectedAt,
            correction.CorrectedBy
        });
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created(
            $"/api/v1/corrections/{correction.Id}",
            ToResponse(correction));
    }


    private static void AddMessages(
        IPlatformDbContext db,
        Guid tenantId,
        Guid farmId,
        CorrectionRecord correction,
        object payload)
    {
        string json = JsonSerializer.Serialize(payload);
        db.SyncChanges.Add(new SyncChange(
            tenantId, farmId, "Correction", correction.Id, "Upsert", json, correction.CorrectedAt));
        db.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(), tenantId, "Record.Corrected", json, correction.CorrectedAt));
    }


    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();


    private static bool ValidText(string? value, int maximumLength) =>
        !string.IsNullOrWhiteSpace(value) && value.Trim().Length <= maximumLength;


    private static CorrectionResponse ToResponse(CorrectionRecord item) => new(
        item.Id,
        item.EntityType,
        item.OriginalEntityId,
        item.ReplacementEntityId,
        item.Reason,
        item.CorrectedAt,
        item.CorrectedBy,
        item.FarmId);

}

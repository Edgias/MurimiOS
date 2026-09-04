using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.CorrectFieldOperation;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CorrectFieldOperationHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IRequestActorContext actor, ITenantContext tenant, TimeProvider timeProvider) : IRequestHandler<CorrectFieldOperationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CorrectFieldOperationCommand message, CancellationToken cancellationToken) => CorrectFieldOperationAsync(message.operationId, message.request, db, scopeAccess, actor, tenant, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> CorrectFieldOperationAsync(
        Guid operationId,
        CorrectFieldOperationRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        IRequestActorContext actor,
        ITenantContext tenant,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (!ValidText(request.OperationType, 80) || !ValidText(request.Reason, 2_000)
            || request.Notes?.Length > 2_000 || request.OccurredAt == default)
            return UseCaseResults.BadRequest(new { code = "invalid_correction", message = "Operation type, occurrence time, and a correction reason are required." });
        var context = await (
            from operation in db.FieldOperations
            join field in db.FarmFields on operation.FieldId equals field.Id
            where operation.Id == operationId
            select new { Operation = operation, field.FarmId })
            .SingleOrDefaultAsync(cancellationToken);
        if (context is null || !await scopeAccess.CanAccessFarmAsync(context.FarmId, cancellationToken))
            return UseCaseResults.NotFound();
        if (context.Operation.SupersededById is not null)
            return UseCaseResults.Conflict(new { code = "already_superseded", replacementId = context.Operation.SupersededById });
        if (context.Operation.OperationType.Equals("InputApplication", StringComparison.OrdinalIgnoreCase))
            return UseCaseResults.Conflict(new
            {
                code = "inventory_adjustment_required",
                message = "Input applications must be corrected through an inventory adjustment so stock lineage remains balanced."
            });

        Guid replacementId = NonEmptyOrNew(request.ReplacementId);
        Guid commandId = NonEmptyOrNew(request.CommandId);
        Guid correctionId = NonEmptyOrNew(request.CorrectionId);
        DateTimeOffset now = timeProvider.GetUtcNow();
        var replacement = new FieldOperationRecord(
            replacementId,
            context.Operation.FieldId,
            context.Operation.BoundaryVersionId,
            request.OperationType,
            request.OccurredAt,
            now,
            commandId,
            request.Notes,
            context.Operation.CropZoneId);
        context.Operation.Supersede(replacementId, request.Reason);
        var correction = new CorrectionRecord(
            correctionId,
            typeof(FieldOperationRecord).FullName!,
            context.Operation.Id,
            replacement.Id,
            request.Reason,
            now,
            actor.Subject,
            context.FarmId);
        db.FieldOperations.Add(replacement);
        db.CorrectionRecords.Add(correction);
        AddMessages(db, tenant.TenantId, context.FarmId, correction, new
        {
            correction.Id,
            OriginalOperationId = context.Operation.Id,
            ReplacementOperationId = replacement.Id,
            replacement.FieldId,
            replacement.CropZoneId,
            replacement.BoundaryVersionId,
            replacement.OperationType,
            replacement.OccurredAt,
            replacement.RecordedAt,
            replacement.Notes,
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

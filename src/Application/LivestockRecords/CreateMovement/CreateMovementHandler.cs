using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateMovement;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;


using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateMovementHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant) : IRequestHandler<CreateMovementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMovementCommand message, CancellationToken cancellationToken) => CreateMovementAsync(message.subjectId, message.request, db, scope, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateMovementAsync(Guid subjectId, CreateLivestockMovementRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, CancellationToken cancellationToken)
    {
        Guid? farmId = await SubjectFarmAsync(subjectId, db, cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        Guid[] holdingIds = new[] { request.FromHoldingId, request.ToHoldingId }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToArray();
        if (holdingIds.Length > 0 && await db.LivestockHoldings.CountAsync(x => holdingIds.Contains(x.Id) && x.FarmId == farmId, cancellationToken) != holdingIds.Length)
            return Bad("holding_not_found", "Movement holdings must belong to the animal's farm.");
        LivestockMovementRecord? latest = await db.LivestockMovements.Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.OccurredAt).FirstOrDefaultAsync(cancellationToken);
        if (latest?.Type is LivestockMovementType.Sale or LivestockMovementType.Death)
            return UseCaseResults.Conflict(new { code = "livestock_lifecycle_closed", message = "No movement can follow a sale or death record." });
        if (latest is not null && request.OccurredAt < latest.OccurredAt)
            return UseCaseResults.Conflict(new { code = "movement_out_of_order", message = "Use a correction workflow for historical movement changes." });
        if (request.FromHoldingId is { } from && latest?.ToHoldingId != from)
            return UseCaseResults.Conflict(new { code = "origin_mismatch", currentHoldingId = latest?.ToHoldingId });
        Guid id = ValidId(request.Id);
        var movement = new LivestockMovementRecord(id, subjectId, request.Type, request.FromHoldingId,
            request.ToHoldingId, request.OccurredAt, request.Reference, null, request.Notes);
        db.LivestockMovements.Add(movement);
        AddOutbox(db, tenant, "LivestockMovement.Recorded", movement.Id, subjectId, request.OccurredAt);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/movements/{id}", ToResponse(movement));
    }

    private static Task<Guid?> SubjectFarmAsync(Guid subjectId, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.LivestockSubjects.Where(x => x.Id == subjectId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);

    private static Guid ValidId(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static void AddOutbox(IPlatformDbContext db, ITenantContext tenant, string type, Guid recordId, Guid subjectId, DateTimeOffset occurredAt) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type,
            JsonSerializer.Serialize(new { recordId, subjectId, occurredAt }), occurredAt));

    private static LivestockIdentifierResponse ToResponse(LivestockIdentifier x) => new(x.Id, x.SubjectId, x.IdentifierNamespace, x.Value, x.ValidFrom, x.ValidTo, x.IsPrimary);

    private static LivestockMovementResponse ToResponse(LivestockMovementRecord x) => new(x.Id, x.SubjectId, x.Type, x.FromHoldingId, x.ToHoldingId, x.OccurredAt, x.Reference, x.Notes);

    private static LivestockHealthEventResponse ToResponse(LivestockHealthEvent x) => new(x.Id, x.SubjectId, x.Type, x.Title, x.OccurredAt, x.Diagnosis, x.Practitioner, x.Notes);

    private static LivestockFeedBatchResponse ToResponse(LivestockFeedBatch x) => new(x.Id, x.FarmId, x.BatchCode, x.FeedName, x.InitialQuantity, x.QuantityOnHand, x.Unit, x.EnteredInitialQuantity, x.EnteredUnit, x.ReceivedOn, x.ExpiresOn, x.Supplier);

    private static LivestockFeedAllocationResponse ToResponse(LivestockFeedAllocation x) => new(x.Id, x.SubjectId, x.FeedBatchId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.OccurredAt, x.Notes);

}

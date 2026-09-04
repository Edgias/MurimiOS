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
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateFeedAllocation;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateFeedAllocationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant) : IRequestHandler<CreateFeedAllocationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateFeedAllocationCommand message, CancellationToken cancellationToken) => CreateFeedAllocationAsync(message.subjectId, message.request, db, scope, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateFeedAllocationAsync(Guid subjectId, CreateLivestockFeedAllocationRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, CancellationToken cancellationToken)
    {
        Guid? farmId = await SubjectFarmAsync(subjectId, db, cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        string lockKey = $"feed-batch:{request.FeedBatchId:N}";
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
        LivestockFeedBatch? batch = await db.LivestockFeedBatches.SingleOrDefaultAsync(x => x.Id == request.FeedBatchId && x.FarmId == farmId, cancellationToken);
        if (batch is null) return UseCaseResults.NotFound();
        if (batch.ExpiresOn is { } expiry && DateOnly.FromDateTime(request.OccurredAt.UtcDateTime) > expiry)
            return UseCaseResults.Conflict(new { code = "feed_batch_expired", expiresOn = expiry });
        MeasuredQuantity measured = MeasurementUnits.Capture(request.Quantity, request.Unit, request.Unit);
        if (!measured.CanonicalUnit.Equals(batch.Unit, StringComparison.OrdinalIgnoreCase)) return Bad("unit_mismatch", $"Feed batch is measured in {batch.Unit}.");
        if (measured.CanonicalValue > batch.QuantityOnHand)
            return UseCaseResults.Conflict(new { code = "insufficient_feed_stock", available = batch.QuantityOnHand, unit = batch.Unit });
        Guid id = ValidId(request.Id); var allocation = new LivestockFeedAllocation(id, subjectId, batch.Id,
            request.Quantity, request.Unit, request.OccurredAt, null, request.Notes);
        batch.Issue(allocation.Quantity); db.LivestockFeedAllocations.Add(allocation);
        AddOutbox(db, tenant, "LivestockFeedAllocation.Recorded", id, subjectId, request.OccurredAt);
        await db.SaveChangesAsync(cancellationToken); await transaction.CommitAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/feed-allocations/{id}", ToResponse(allocation));
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

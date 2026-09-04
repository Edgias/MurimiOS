using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateHealthEvent;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;


using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;

public sealed class CreateHealthEventHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateHealthEventCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateHealthEventCommand message, CancellationToken cancellationToken) => CreateHealthEventAsync(message.subjectId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateHealthEventAsync(Guid subjectId, CreateLivestockHealthEventRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        Guid id = ValidId(request.Id); var health = new LivestockHealthEvent(id, subjectId, request.Type, request.Title,
            request.OccurredAt, request.Diagnosis, request.Practitioner, null, request.Notes);
        db.LivestockHealthEvents.Add(health); await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/health-events/{id}", ToResponse(health));
    }


    private static async Task<bool> CanAccessSubjectAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        Guid? farmId = await SubjectFarmAsync(subjectId, db, cancellationToken);
        return farmId is not null && await scope.CanAccessFarmAsync(farmId.Value, cancellationToken);
    }

    private static Task<Guid?> SubjectFarmAsync(Guid subjectId, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.LivestockSubjects.Where(x => x.Id == subjectId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);

    private static Guid ValidId(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static LivestockIdentifierResponse ToResponse(LivestockIdentifier x) => new(x.Id, x.SubjectId, x.IdentifierNamespace, x.Value, x.ValidFrom, x.ValidTo, x.IsPrimary);

    private static LivestockMovementResponse ToResponse(LivestockMovementRecord x) => new(x.Id, x.SubjectId, x.Type, x.FromHoldingId, x.ToHoldingId, x.OccurredAt, x.Reference, x.Notes);

    private static LivestockHealthEventResponse ToResponse(LivestockHealthEvent x) => new(x.Id, x.SubjectId, x.Type, x.Title, x.OccurredAt, x.Diagnosis, x.Practitioner, x.Notes);

    private static LivestockFeedBatchResponse ToResponse(LivestockFeedBatch x) => new(x.Id, x.FarmId, x.BatchCode, x.FeedName, x.InitialQuantity, x.QuantityOnHand, x.Unit, x.EnteredInitialQuantity, x.EnteredUnit, x.ReceivedOn, x.ExpiresOn, x.Supplier);

    private static LivestockFeedAllocationResponse ToResponse(LivestockFeedAllocation x) => new(x.Id, x.SubjectId, x.FeedBatchId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.OccurredAt, x.Notes);

}

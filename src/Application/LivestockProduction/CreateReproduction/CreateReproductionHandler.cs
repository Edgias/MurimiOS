using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateReproduction;
using LivestockOffspringLink = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockOffspringLink;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using OffspringOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.OffspringOutcome;
using ReproductionEventType = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionEventType;
using ReproductionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionOutcome;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockSex = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSex;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateReproductionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor) : IRequestHandler<CreateReproductionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateReproductionCommand message, CancellationToken cancellationToken) => CreateReproductionAsync(message.subjectId, message.request, db, scope, tenant, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateReproductionAsync(Guid subjectId, CreateReproductionEventRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await AccessibleSubject(subjectId, db, scope, cancellationToken);
        if (subject is null) return UseCaseResults.NotFound();
        if (subject.Kind != LivestockSubjectKind.Individual) return Bad("individual_required", "Reproduction events require an individual animal.");
        LivestockBiologicalProfile? profile = await db.LivestockBiologicalProfiles.SingleOrDefaultAsync(x => x.SubjectId == subjectId, cancellationToken);
        if (request.Type is not ReproductionEventType.Weaning && profile?.Sex != LivestockSex.Female)
            return Bad("female_profile_required", "Breeding, pregnancy and parturition require a female biological profile.");
        LivestockSubject? related = null;
        if (request.RelatedSubjectId is { } relatedId)
        {
            related = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == relatedId && x.FarmId == subject.FarmId
                && x.Kind == LivestockSubjectKind.Individual && x.Species == subject.Species, cancellationToken);
            if (related is null) return Bad("related_subject_invalid", "The related animal must be an individual of the same species and farm.");
            if (request.Type == ReproductionEventType.Breeding)
            {
                LivestockSex? sex = await db.LivestockBiologicalProfiles.Where(x => x.SubjectId == relatedId)
                    .Select(x => (LivestockSex?)x.Sex).SingleOrDefaultAsync(cancellationToken);
                if (sex != LivestockSex.Male) return Bad("male_profile_required", "A recorded sire must have a male biological profile.");
            }
        }
        LivestockReproductionEvent? breeding = null;
        if (request.BreedingEventId is { } breedingId)
        {
            breeding = await db.LivestockReproductionEvents.SingleOrDefaultAsync(x => x.Id == breedingId
                && x.SubjectId == subjectId && x.Type == ReproductionEventType.Breeding, cancellationToken);
            if (breeding is null) return Bad("breeding_event_invalid", "The breeding event must belong to the same dam.");
            if (breeding.OccurredAt > request.OccurredAt) return Bad("reproduction_sequence_invalid", "An outcome cannot precede breeding.");
        }
        IReadOnlyList<CreateOffspringLinkRequest> requestedOffspring = request.Offspring ?? [];
        if (request.Type != ReproductionEventType.Parturition && requestedOffspring.Count > 0)
            return Bad("offspring_not_applicable", "Offspring links are only valid for parturition.");
        Guid[] offspringIds = requestedOffspring.Select(x => x.SubjectId).Distinct().ToArray();
        if (offspringIds.Length != requestedOffspring.Count) return Bad("offspring_duplicate", "Each offspring may be linked once.");
        LivestockSubject[] offspringSubjects = offspringIds.Length == 0 ? [] : await db.LivestockSubjects
            .Where(x => offspringIds.Contains(x.Id) && x.FarmId == subject.FarmId && x.Kind == LivestockSubjectKind.Individual
                && x.Species == subject.Species).ToArrayAsync(cancellationToken);
        if (offspringSubjects.Length != offspringIds.Length) return Bad("offspring_scope_invalid", "Offspring must be individual animals of the same species and farm.");
        if (request.Type == ReproductionEventType.Parturition && request.OffspringCount < requestedOffspring.Count)
            return Bad("offspring_count_invalid", "Registered offspring cannot exceed the total offspring count.");

        try
        {
            var record = new LivestockReproductionEvent(Guid.NewGuid(), subject.FarmId, subjectId, request.Type,
                request.Outcome, request.OccurredAt, request.RelatedSubjectId, request.BreedingEventId, request.Method,
                request.ExpectedDueOn, request.OffspringCount, request.Weight, request.WeightUnit, request.Practitioner, request.Notes);
            db.LivestockReproductionEvents.Add(record);
            foreach (CreateOffspringLinkRequest item in requestedOffspring)
            {
                db.LivestockOffspringLinks.Add(new LivestockOffspringLink(Guid.NewGuid(), record.Id, item.SubjectId,
                    item.Outcome, item.BirthWeight, item.BirthWeightUnit));
                if (!await db.LivestockBiologicalProfiles.AnyAsync(x => x.SubjectId == item.SubjectId, cancellationToken))
                    db.LivestockBiologicalProfiles.Add(new LivestockBiologicalProfile(Guid.NewGuid(), item.SubjectId,
                        item.Sex, DateOnly.FromDateTime(request.OccurredAt.UtcDateTime), item.Breed, subjectId,
                        breeding?.RelatedSubjectId));
            }
            AddEvents(db, tenant, subject.FarmId, "LivestockReproduction", record.Id, "Recorded",
                new { record.Id, record.SubjectId, record.Type, record.Outcome, record.OccurredAt }, record.OccurredAt);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/reproduction-events/{record.Id}",
                ReproductionResponse(record, db.LivestockOffspringLinks.Local.Where(x => x.ParturitionEventId == record.Id)));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Bad("reproduction_event_invalid", exception.Message); }
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }


    private static void AddEvents(IPlatformDbContext db, ITenantContext tenant, Guid farmId, string entityType,
        Guid entityId, string operation, object value, DateTimeOffset occurredAt)
    {
        string json = JsonSerializer.Serialize(value);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, operation, json, occurredAt));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"{entityType}.{operation}", json, occurredAt));
    }

    private static object ReproductionResponse(LivestockReproductionEvent x, IEnumerable<LivestockOffspringLink> links) => new
    {
        x.Id, x.FarmId, x.SubjectId, x.Type, x.Outcome, x.OccurredAt, x.RelatedSubjectId, x.BreedingEventId,
        x.Method, x.ExpectedDueOn, x.OffspringCount, x.WeightKg, x.EnteredWeight, x.EnteredWeightUnit,
        x.Practitioner, x.Notes, Offspring = links.Select(link => new { link.Id, link.OffspringSubjectId, link.Outcome,
            link.BirthWeightKg, link.EnteredBirthWeight, link.EnteredBirthWeightUnit }).ToArray()
    };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

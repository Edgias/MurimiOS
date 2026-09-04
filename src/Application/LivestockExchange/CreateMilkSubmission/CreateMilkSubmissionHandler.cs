using EdgIQ.MurimiOS.Application.Common.Livestock;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.CreateMilkSubmission;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateMilkSubmissionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<CreateMilkSubmissionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMilkSubmissionCommand message, CancellationToken cancellationToken) => CreateMilkSubmissionAsync(message.milkRecordId, message.request, db, scope, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateMilkSubmissionAsync(Guid milkRecordId,
        CreateLivestockExchangeSubmissionRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, CancellationToken cancellationToken)
    {
        var context = await (from milk in db.LivestockMilkRecords
            join subject in db.LivestockSubjects on milk.SubjectId equals subject.Id
            join biological in db.LivestockBiologicalProfiles on subject.Id equals biological.SubjectId
            where milk.Id == milkRecordId
            select new { Milk = milk, Subject = subject, Biological = biological }).SingleOrDefaultAsync(cancellationToken);
        if (context is null || !await scope.CanAccessFarmAsync(context.Milk.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        string? identifier = await ActiveIdentifierAsync(context.Subject.Id, context.Milk.MilkedAt, db, cancellationToken)
            ?? context.Subject.OfficialIdentifier;
        if (string.IsNullOrWhiteSpace(identifier))
            return Bad("milk_recording_identifier_missing", "An active official animal identifier is required.");
        LivestockRegulatoryProfile? profile = await ResolveProfileAsync(request.ProfileId, LivestockRegulatoryDefaults.IcarCattleMilkProfileKey,
            LivestockRegulatoryExchangeType.MilkRecording, context.Subject.Species, context.Milk.MilkedAt, db, cancellationToken);
        if (profile is null)
            return Bad("milk_recording_profile_invalid", "The regulatory profile was not found or does not apply to this exchange, species, and event date.");
        if (profile.ProviderCode != LivestockRegulatoryDefaults.IcarAdeProvider)
            return Bad("milk_recording_provider_unsupported", "The current milk-recording adapter supports ICAR ADE profiles.");
        if (await db.LivestockExchangeSubmissions.AnyAsync(x => x.ExchangeType == LivestockRegulatoryExchangeType.MilkRecording
            && x.SourceRecordId == milkRecordId && x.ProviderCode == profile.ProviderCode, cancellationToken))
            return Conflict("milk_recording_submission_exists", "A submission already exists for this milk record and provider.");

        Guid id = Id(request.Id);
        DateTimeOffset now = clock.GetUtcNow();
        string payload = JsonSerializer.Serialize(new
        {
            standard = "ICAR-ADE", version = profile.SchemaVersion, messageType = "milkingObservation",
            messageId = id, createdAt = now, animalIdentifier = identifier,
            observation = new { context.Milk.MilkedAt, context.Milk.Session,
                quantityLitres = context.Milk.QuantityLitres, discardedLitres = context.Milk.DiscardedLitres,
                saleableLitres = context.Milk.SaleableLitres, context.Milk.FatPercent, context.Milk.ProteinPercent,
                context.Milk.QualityGrade, context.Milk.WithdrawalHoldUntil }
        });
        var submission = NewSubmission(id, profile, context.Milk.FarmId, context.Subject.Id, milkRecordId,
            LivestockRegulatoryExchangeType.MilkRecording, payload, now, actor.Subject);
        db.LivestockExchangeSubmissions.Add(submission);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockExchange.MilkRecordingCreated",
            JsonSerializer.Serialize(new { submission.Id, submission.SubjectId, submission.ProfileId }), now));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/exchange-submissions/{id}", SubmissionResponse(submission, []));
    }


    private static async Task<LivestockRegulatoryProfile?> ResolveProfileAsync(Guid? profileId, string defaultKey,
        LivestockRegulatoryExchangeType type, string species, DateTimeOffset eventAt, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        DateOnly eventDate = DateOnly.FromDateTime(eventAt.UtcDateTime);
        if (profileId is null && !await db.LivestockRegulatoryProfiles
            .AnyAsync(x => x.ProfileKey == defaultKey && x.ProfileVersion == 1, cancellationToken))
            await LivestockRegulatoryDefaults.EnsureDefaultsAsync(db, cancellationToken);
        LivestockRegulatoryProfile? profile = profileId is { } id
            ? await db.LivestockRegulatoryProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            : await db.LivestockRegulatoryProfiles.AsNoTracking().Where(x => x.ProfileKey == defaultKey
                && x.EffectiveFrom <= eventDate && (x.EffectiveTo == null || x.EffectiveTo >= eventDate))
                .OrderByDescending(x => x.ProfileVersion).FirstOrDefaultAsync(cancellationToken);
        if (profile is null) return null;
        if (profile.ExchangeType != type || !profile.Species.Equals(species, StringComparison.OrdinalIgnoreCase)
            || !profile.AppliesOn(eventAt))
            return null;
        return profile;
    }


    private static LivestockExchangeSubmission NewSubmission(Guid id, LivestockRegulatoryProfile profile,
        Guid farmId, Guid subjectId, Guid sourceId, LivestockRegulatoryExchangeType type, string payload,
        DateTimeOffset createdAt, string actor) => new(id, profile.Id, farmId, subjectId, sourceId, type,
            profile.ProviderCode, profile.SchemaVersion, profile.MediaType, payload, Sha256(payload), createdAt,
            actor, profile.ReportingDeadlineHours is { } hours ? createdAt.AddHours(hours) : null);


    private static async Task<string?> ActiveIdentifierAsync(Guid subjectId, DateTimeOffset at,
        IPlatformDbContext db, CancellationToken cancellationToken) => await db.LivestockIdentifiers
        .Where(x => x.SubjectId == subjectId && x.ValidFrom <= at && (x.ValidTo == null || x.ValidTo >= at))
        .OrderByDescending(x => x.IsPrimary).ThenByDescending(x => x.ValidFrom).Select(x => x.Value)
        .FirstOrDefaultAsync(cancellationToken);

    private static object SubmissionResponse(LivestockExchangeSubmission x, IReadOnlyCollection<LivestockExchangeAttempt> attempts) => new
    {
        x.Id, x.ProfileId, x.FarmId, x.SubjectId, x.SourceRecordId, x.ExchangeType, x.ProviderCode,
        x.SchemaVersion, x.MediaType, payload = JsonSerializer.Deserialize<JsonElement>(x.PayloadJson),
        x.PayloadSha256, x.CreatedAt, x.CreatedBy, x.ReportingDueAt, x.Status, x.FirstSubmittedAt,
        x.DecidedAt, x.ExternalReference, x.RejectionCode, x.RejectionMessage,
        attempts = attempts.Select(a => new { a.Id, a.AttemptNumber, a.Outcome, a.StartedAt, a.CompletedAt,
            a.HttpStatus, a.ExternalReference, a.ErrorCode, a.ErrorMessage }).ToArray()
    };

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static string Sha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

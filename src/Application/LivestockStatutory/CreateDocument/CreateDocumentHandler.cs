using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.CreateDocument;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed partial class CreateDocumentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<CreateDocumentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateDocumentCommand message, CancellationToken cancellationToken) => CreateDocumentAsync(message.movementId, message.request, db, scope, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateDocumentAsync(Guid movementId, CreateStatutoryMovementDocumentRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, CancellationToken cancellationToken)
    {
        var context = await (from movement in db.LivestockMovements
            join subject in db.LivestockSubjects on movement.SubjectId equals subject.Id
            join farm in db.Farms on subject.FarmId equals farm.Id
            where movement.Id == movementId
            select new { Movement = movement, Subject = subject, Farm = farm }).SingleOrDefaultAsync(cancellationToken);
        if (context is null || !await scope.CanAccessFarmAsync(context.Farm.Id, cancellationToken)) return UseCaseResults.NotFound();
        if (context.Subject.Kind != LivestockSubjectKind.Individual || !context.Subject.Species.Equals("cattle", StringComparison.OrdinalIgnoreCase))
            return Bad("statutory_subject_invalid", "The launch statutory movement adapter requires an individual cattle record.");
        if (context.Movement.FromHoldingId is null || context.Movement.ToHoldingId is null)
            return Bad("statutory_holdings_required", "The movement must identify both departure and destination holdings.");
        if (request.ProfileId is null && !await db.LivestockRegulatoryProfiles
            .AnyAsync(x => x.ProfileKey == global::EdgIQ.MurimiOS.Application.Common.Livestock.LivestockRegulatoryDefaults.ScotlandCattleMovementProfileKey
                && x.ProfileVersion == 1, cancellationToken))
            await global::EdgIQ.MurimiOS.Application.Common.Livestock.LivestockRegulatoryDefaults.EnsureDefaultsAsync(db, cancellationToken);
        DateOnly movementDate = DateOnly.FromDateTime(context.Movement.OccurredAt.UtcDateTime);
        LivestockRegulatoryProfile? profile = request.ProfileId is { } selectedProfileId
            ? await db.LivestockRegulatoryProfiles.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == selectedProfileId, cancellationToken)
            : await db.LivestockRegulatoryProfiles.AsNoTracking()
                .Where(x => x.ProfileKey == global::EdgIQ.MurimiOS.Application.Common.Livestock.LivestockRegulatoryDefaults.ScotlandCattleMovementProfileKey
                    && x.EffectiveFrom <= movementDate && (x.EffectiveTo == null || x.EffectiveTo >= movementDate))
                .OrderByDescending(x => x.ProfileVersion).FirstOrDefaultAsync(cancellationToken);
        if (profile is null || profile.ExchangeType != LivestockRegulatoryExchangeType.Movement
            || !profile.Species.Equals(context.Subject.Species, StringComparison.OrdinalIgnoreCase)
            || !profile.AppliesOn(context.Movement.OccurredAt))
            return Bad("statutory_profile_invalid", "The selected regulatory profile does not apply to this movement, species, or event date.");
        if (profile.ProviderCode != ScotEidProvider)
            return Bad("statutory_provider_unsupported", "The current statutory movement adapter supports ScotEID profiles.");
        if (!string.IsNullOrWhiteSpace(request.SchemaVersion)
            && !request.SchemaVersion.Equals(profile.SchemaVersion, StringComparison.OrdinalIgnoreCase))
            return Bad("statutory_schema_mismatch", "Schema version is fixed by the versioned regulatory profile.");
        if (await db.LivestockStatutoryMovementDocuments.AnyAsync(x => x.MovementId == movementId && x.ProviderCode == profile.ProviderCode, cancellationToken))
            return Conflict("statutory_document_exists", "A document already exists for this movement and provider.");

        LivestockHolding[] holdings = await db.LivestockHoldings.Where(x => x.Id == context.Movement.FromHoldingId || x.Id == context.Movement.ToHoldingId)
            .ToArrayAsync(cancellationToken);
        LivestockHolding? from = holdings.SingleOrDefault(x => x.Id == context.Movement.FromHoldingId);
        LivestockHolding? to = holdings.SingleOrDefault(x => x.Id == context.Movement.ToHoldingId);
        if (from is null || to is null || !CphPattern().IsMatch(from.Code) || !CphPattern().IsMatch(to.Code))
            return Bad("statutory_cph_invalid", "Departure and destination holding codes must use the CPH format 00/000/0000.");

        string? animalId = await db.LivestockIdentifiers.Where(x => x.SubjectId == context.Subject.Id
                && x.ValidFrom <= context.Movement.OccurredAt && (x.ValidTo == null || x.ValidTo >= context.Movement.OccurredAt))
            .OrderByDescending(x => x.IsPrimary).Select(x => x.Value).FirstOrDefaultAsync(cancellationToken)
            ?? context.Subject.OfficialIdentifier;
        if (string.IsNullOrWhiteSpace(animalId)) return Bad("statutory_identifier_missing", "An active official cattle identifier is required.");

        DateTimeOffset now = clock.GetUtcNow();
        Guid id = Id(request.Id);
        string documentNumber = string.IsNullOrWhiteSpace(request.DocumentNumber)
            ? $"SM-{context.Movement.OccurredAt:yyyyMMdd}-{movementId:N}"[..32]
            : request.DocumentNumber.Trim();
        var snapshot = new StatutoryMovementSnapshot(1, animalId.Trim(), from.Code, to.Code, to.Name,
            DateOnly.FromDateTime(context.Movement.OccurredAt.UtcDateTime),
            DateOnly.FromDateTime(context.Movement.OccurredAt.UtcDateTime), context.Movement.Reference ?? documentNumber,
            movementId, context.Subject.Id, context.Farm.Id);
        string snapshotJson = JsonSerializer.Serialize(snapshot);
        string renderedDocument = RenderMovementDocument(profile, snapshot, documentNumber);
        string hash = Sha256(renderedDocument);
        var document = new LivestockStatutoryMovementDocument(id, movementId, context.Subject.Id, context.Farm.Id,
            profile.Jurisdiction, profile.ProviderCode, profile.SchemaVersion, documentNumber,
            snapshotJson, hash, now, actor.Subject,
            context.Movement.OccurredAt.AddHours(profile.ReportingDeadlineHours!.Value),
            context.Movement.OccurredAt.AddHours(profile.RegisterDeadlineHours!.Value), profile.Id,
            profile.MediaType, renderedDocument);
        db.LivestockStatutoryMovementDocuments.Add(document);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockStatutory.DocumentCreated",
            JsonSerializer.Serialize(new { document.Id, document.MovementId, document.ProviderCode, document.ReportingDueAt }), now));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/statutory-documents/{id}", ToResponse(document, []));
    }

    private const string ScotEidProvider = "scoteid";

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string Sha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static string RenderMovementDocument(LivestockRegulatoryProfile profile,
        StatutoryMovementSnapshot snapshot, string documentNumber)
    {
        var document = new XDocument(new XElement("LivestockMovementDocument",
            new XAttribute("jurisdiction", profile.Jurisdiction),
            new XAttribute("provider", profile.ProviderCode),
            new XAttribute("schemaVersion", profile.SchemaVersion),
            new XElement("DocumentNumber", documentNumber),
            new XElement("AnimalIdentifier", snapshot.AnimalId),
            new XElement("DepartureHolding", snapshot.DepartureMainCph),
            new XElement("DestinationHolding", snapshot.DestinationMainCph),
            new XElement("DestinationLocation", snapshot.DestinationLocation),
            new XElement("DepartureDate", snapshot.DepartureDate.ToString("yyyy-MM-dd")),
            new XElement("MoveDate", snapshot.MoveDate.ToString("yyyy-MM-dd")),
            new XElement("UserReference", snapshot.UserReference)));
        return document.ToString(SaveOptions.DisableFormatting);
    }

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


    [GeneratedRegex(@"^\d{2}/\d{3}/\d{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex CphPattern();

}

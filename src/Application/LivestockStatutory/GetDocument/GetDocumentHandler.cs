using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.GetDocument;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;

public sealed class GetDocumentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetDocumentQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetDocumentQuery message, CancellationToken cancellationToken) => GetDocumentAsync(message.documentId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetDocumentAsync(Guid documentId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockStatutoryMovementDocument? document = await db.LivestockStatutoryMovementDocuments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == documentId, cancellationToken);
        if (document is null || !await scope.CanAccessFarmAsync(document.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockStatutorySubmissionAttempt[] attempts = await db.LivestockStatutorySubmissionAttempts.AsNoTracking()
            .Where(x => x.DocumentId == documentId).OrderBy(x => x.AttemptNumber).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(ToResponse(document, attempts));
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

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

}

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
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.ListCropFeedTransfers;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;

public sealed class ListCropFeedTransfersHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListCropFeedTransfersQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCropFeedTransfersQuery message, CancellationToken cancellationToken) => ListCropFeedTransfersAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListCropFeedTransfersAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        CropFeedTransfer[] rows = await db.CropFeedTransfers.AsNoTracking().Where(x => x.FarmId == farmId)
            .OrderByDescending(x => x.TransferredAt).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToResponse).ToArray());
    }

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

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

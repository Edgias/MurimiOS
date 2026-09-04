using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.SubmitDocument;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class SubmitDocumentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IIntegrationCredentialProtector protector, IScotEidMovementAdapter adapter, ITenantContext tenant, TimeProvider clock) : IRequestHandler<SubmitDocumentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(SubmitDocumentCommand message, CancellationToken cancellationToken) => SubmitDocumentAsync(message.documentId, message.request, db, scope, protector, adapter, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> SubmitDocumentAsync(Guid documentId, SubmitStatutoryMovementDocumentRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IIntegrationCredentialProtector protector,
        IScotEidMovementAdapter adapter, ITenantContext tenant, TimeProvider clock, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
        {
            string lockKey = $"statutory-document:{documentId:N}";
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0));", cancellationToken);
        }
        LivestockStatutoryMovementDocument? document = await db.LivestockStatutoryMovementDocuments
            .SingleOrDefaultAsync(x => x.Id == documentId, cancellationToken);
        if (document is null || !await scope.CanAccessFarmAsync(document.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (document.Status != StatutoryMovementDocumentStatus.Ready)
            return Conflict("statutory_document_final", "An accepted or rejected document cannot be resubmitted; create a replacement document after correction.");
        IntegrationConnection? connection = await db.IntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.ConnectionId
            && x.ProviderCode == document.ProviderCode && x.Category == IntegrationCategory.Regulatory
            && x.Status == IntegrationConnectionStatus.Active, cancellationToken);
        if (connection is null) return Bad("scoteid_connection_invalid", "Select an active ScotEID regulatory connection.");
        DateTimeOffset started = clock.GetUtcNow();
        IntegrationCredential? credential = await db.IntegrationCredentials.AsNoTracking().Where(x => x.ConnectionId == connection.Id
                && x.Status == IntegrationCredentialStatus.Active && x.ValidFrom <= started && (x.ExpiresAt == null || x.ExpiresAt > started))
            .OrderByDescending(x => x.CredentialVersion).FirstOrDefaultAsync(cancellationToken);
        if (credential is null) return Bad("scoteid_credential_missing", "The ScotEID connection has no active credential.");

        StatutoryMovementSnapshot? snapshot;
        ScotEidCredentials? secrets;
        ScotEidOptions options;
        try
        {
            snapshot = JsonSerializer.Deserialize<StatutoryMovementSnapshot>(document.SnapshotJson);
            secrets = JsonSerializer.Deserialize<ScotEidCredentials>(protector.Unprotect(credential.ProtectedReference), JsonOptions);
            options = ParseOptions(connection.ConfigurationJson, document.SchemaVersion);
            if (snapshot is null || secrets is null || string.IsNullOrWhiteSpace(secrets.Username)
                || string.IsNullOrWhiteSpace(secrets.Password) || string.IsNullOrWhiteSpace(secrets.ApplicationKey))
                throw new JsonException("The document snapshot or credential JSON is incomplete.");
        }
        catch (Exception ex) when (ex is JsonException or FormatException or System.Security.Cryptography.CryptographicException or ArgumentException)
        {
            return Bad("scoteid_configuration_invalid", ex.Message);
        }

        var submission = new ScotEidMovementSubmission(snapshot.AnimalId, snapshot.DepartureMainCph,
            snapshot.DestinationMainCph, snapshot.DestinationLocation, snapshot.DepartureDate,
            snapshot.MoveDate, snapshot.UserReference);
        ScotEidSubmissionResult result = await adapter.SubmitAsync(options, secrets, submission, cancellationToken);
        DateTimeOffset completed = clock.GetUtcNow();
        int number = (await db.LivestockStatutorySubmissionAttempts.Where(x => x.DocumentId == documentId)
            .MaxAsync(x => (int?)x.AttemptNumber, cancellationToken) ?? 0) + 1;
        StatutorySubmissionOutcome outcome = result.Accepted ? StatutorySubmissionOutcome.Accepted
            : result.Retriable ? StatutorySubmissionOutcome.RetriableFailure : StatutorySubmissionOutcome.Rejected;
        var attempt = new LivestockStatutorySubmissionAttempt(Guid.NewGuid(), documentId, number, outcome,
            started, completed, result.RequestSha256, result.HttpStatus, result.MovementReference,
            result.ResponseJson, result.ErrorCode, result.ErrorMessage);
        db.LivestockStatutorySubmissionAttempts.Add(attempt);
        if (result.Accepted) document.Accept(started, completed, result.MovementReference!);
        else if (!result.Retriable) document.Reject(started, completed, result.ErrorCode!, result.ErrorMessage!);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockStatutory.SubmissionCompleted",
            JsonSerializer.Serialize(new { documentId = document.Id, attemptId = attempt.Id, attempt.Outcome, result.MovementReference, result.ErrorCode }), completed));
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result.Retriable
            ? UseCaseResults.Json(new { code = result.ErrorCode, message = result.ErrorMessage, attemptId = attempt.Id, retryable = true }, statusCode: 503)
            : UseCaseResults.Ok(ToResponse(document, [attempt]));
    }


    private static ScotEidOptions ParseOptions(string json, string schemaVersion)
    {
        using JsonDocument configuration = JsonDocument.Parse(json);
        JsonElement root = configuration.RootElement;
        string endpointText = String(root, "endpoint") ?? "https://api.test.scoteid.com/api";
        if (!Uri.TryCreate(endpointText, UriKind.Absolute, out Uri? endpoint) || endpoint.Scheme != Uri.UriSchemeHttps)
            throw new FormatException("The ScotEID endpoint must be an absolute HTTPS URL.");
        return new ScotEidOptions(endpoint, String(root, "applicationName") ?? "MurimiOS",
            String(root, "applicationVersion") ?? "1.0", String(root, "schemaVersion") ?? schemaVersion,
            String(root, "soapAction") ?? "SMECreateCattleMovements");
    }


    private static string? String(JsonElement root, string name) => root.TryGetProperty(name, out JsonElement value)
        && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

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

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

}

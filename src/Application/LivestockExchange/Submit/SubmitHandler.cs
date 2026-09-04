using EdgIQ.MurimiOS.Application.Common.Livestock;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.Submit;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeAttemptOutcome = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttemptOutcome;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class SubmitHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IIntegrationCredentialProtector protector, IIcarAdeExchangeAdapter adapter, ITenantContext tenant, TimeProvider clock) : IRequestHandler<SubmitCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(SubmitCommand message, CancellationToken cancellationToken) => SubmitAsync(message.submissionId, message.request, db, scope, protector, adapter, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> SubmitAsync(Guid submissionId, SubmitLivestockExchangeRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IIntegrationCredentialProtector protector,
        IIcarAdeExchangeAdapter adapter, ITenantContext tenant, TimeProvider clock, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        await LockAsync(db, $"livestock-exchange:{submissionId:N}", cancellationToken);
        LivestockExchangeSubmission? submission = await db.LivestockExchangeSubmissions
            .SingleOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
        if (submission is null || !await scope.CanAccessFarmAsync(submission.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (submission.Status is LivestockExchangeSubmissionStatus.Accepted or LivestockExchangeSubmissionStatus.Rejected)
            return Conflict("livestock_exchange_final", "A final submission cannot be resubmitted.");
        if (submission.ProviderCode != LivestockRegulatoryDefaults.IcarAdeProvider)
            return Bad("livestock_exchange_provider_unsupported", "No outbound adapter is registered for this profile provider.");
        IntegrationConnection? connection = await db.IntegrationConnections.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.ConnectionId && x.ProviderCode == submission.ProviderCode
                && x.Category == IntegrationCategory.Regulatory && x.Status == IntegrationConnectionStatus.Active, cancellationToken);
        if (connection is null) return Bad("livestock_exchange_connection_invalid", "Select an active regulatory connection for this provider.");
        DateTimeOffset started = clock.GetUtcNow();
        IntegrationCredential? credential = await ActiveCredentialAsync(connection.Id, started, db, cancellationToken);
        if (credential is null) return Bad("livestock_exchange_credential_missing", "The connection has no active credential.");
        IcarAdeCredentials? secrets;
        IcarAdeOptions options;
        try
        {
            secrets = JsonSerializer.Deserialize<IcarAdeCredentials>(protector.Unprotect(credential.ProtectedReference), JsonOptions);
            options = ParseIcarOptions(connection.ConfigurationJson, submission.SchemaVersion);
            if (secrets is null || string.IsNullOrWhiteSpace(secrets.BearerToken) && string.IsNullOrWhiteSpace(secrets.ApiKey))
                throw new JsonException("A bearer token or API key is required.");
        }
        catch (Exception ex) when (ex is JsonException or FormatException or System.Security.Cryptography.CryptographicException)
        {
            return Bad("livestock_exchange_configuration_invalid", ex.Message);
        }

        IcarAdeExchangeResult result = await adapter.SubmitAsync(options, secrets, submission.Id,
            submission.PayloadJson, cancellationToken);
        DateTimeOffset completed = clock.GetUtcNow();
        int number = (await db.LivestockExchangeAttempts.Where(x => x.SubmissionId == submissionId)
            .MaxAsync(x => (int?)x.AttemptNumber, cancellationToken) ?? 0) + 1;
        LivestockExchangeAttemptOutcome outcome = result.Accepted ? LivestockExchangeAttemptOutcome.Accepted
            : result.Pending ? LivestockExchangeAttemptOutcome.Submitted
            : result.Retriable ? LivestockExchangeAttemptOutcome.RetriableFailure : LivestockExchangeAttemptOutcome.Rejected;
        var attempt = new LivestockExchangeAttempt(Guid.NewGuid(), submissionId, number, outcome, started, completed,
            result.RequestSha256, result.HttpStatus, result.ExternalReference, result.ResponseJson,
            result.ErrorCode, result.ErrorMessage);
        db.LivestockExchangeAttempts.Add(attempt);
        if (result.Accepted) submission.Accept(completed, result.ExternalReference!);
        else if (result.Pending) submission.MarkSubmitted(completed, result.ExternalReference!);
        else if (!result.Retriable) submission.Reject(completed, result.ErrorCode!, result.ErrorMessage!);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockExchange.SubmissionAttempted",
            JsonSerializer.Serialize(new { submissionId = submission.Id, attemptId = attempt.Id,
                attempt.Outcome, result.ExternalReference, result.ErrorCode }), completed));
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result.Retriable
            ? UseCaseResults.Json(new { code = result.ErrorCode, message = result.ErrorMessage, attemptId = attempt.Id, retryable = true }, statusCode: 503)
            : UseCaseResults.Ok(SubmissionResponse(submission, [attempt]));
    }


    private static async Task<IntegrationCredential?> ActiveCredentialAsync(Guid connectionId, DateTimeOffset at,
        IPlatformDbContext db, CancellationToken cancellationToken) => await db.IntegrationCredentials.AsNoTracking()
        .Where(x => x.ConnectionId == connectionId && x.Status == IntegrationCredentialStatus.Active
            && x.ValidFrom <= at && (x.ExpiresAt == null || x.ExpiresAt > at))
        .OrderByDescending(x => x.CredentialVersion).FirstOrDefaultAsync(cancellationToken);


    private static IcarAdeOptions ParseIcarOptions(string configurationJson, string schemaVersion)
    {
        using JsonDocument configuration = JsonDocument.Parse(configurationJson);
        JsonElement root = configuration.RootElement;
        string endpointText = String(root, "endpoint") ?? throw new FormatException("The ICAR ADE endpoint is required.");
        if (!Uri.TryCreate(endpointText, UriKind.Absolute, out Uri? endpoint) || endpoint.Scheme != Uri.UriSchemeHttps)
            throw new FormatException("The ICAR ADE endpoint must be an absolute HTTPS URL.");
        string organisationId = String(root, "organisationId")
            ?? throw new FormatException("The ICAR ADE organisationId is required.");
        return new IcarAdeOptions(endpoint, organisationId, String(root, "schemaVersion") ?? schemaVersion);
    }


    private static async Task LockAsync(IPlatformDbContext db, string key, CancellationToken cancellationToken)
    {
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0));", cancellationToken);
    }

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

    private static string? String(JsonElement root, string name) => root.TryGetProperty(name, out JsonElement value)
        && value.ValueKind == JsonValueKind.String ? value.GetString()?.Trim() : null;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

}

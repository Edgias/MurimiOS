using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;

public sealed class LivestockStatutorySubmissionAttempt : TenantEntity
{
    private LivestockStatutorySubmissionAttempt() { }

    public LivestockStatutorySubmissionAttempt(Guid id, Guid documentId, int attemptNumber,
        StatutorySubmissionOutcome outcome, DateTimeOffset startedAt, DateTimeOffset completedAt,
        string requestSha256, int? httpStatus, string? externalReference, string responseJson,
        string? errorCode, string? errorMessage) : base(id)
    {
        if (documentId == Guid.Empty || attemptNumber <= 0 || !Enum.IsDefined(outcome) || completedAt < startedAt)
            throw new ArgumentException("A valid document, attempt number, outcome, and time range are required.");
        DocumentId = documentId;
        AttemptNumber = attemptNumber;
        Outcome = outcome;
        StartedAt = startedAt.ToUniversalTime();
        CompletedAt = completedAt.ToUniversalTime();
        RequestSha256 = requestSha256.Trim().ToLowerInvariant();
        if (RequestSha256.Length != 64 || RequestSha256.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 request hash is required.");
        if (httpStatus is < 100 or > 599) throw new ArgumentOutOfRangeException(nameof(httpStatus));
        HttpStatus = httpStatus;
        ExternalReference = Optional(externalReference, 160);
        ResponseJson = string.IsNullOrWhiteSpace(responseJson) ? "{}" : responseJson.Trim();
        if (ResponseJson.Length > 64_000) throw new ArgumentException("Response JSON is too large.");
        ErrorCode = Optional(errorCode, 80);
        ErrorMessage = Optional(errorMessage, 2_000);
    }

    public Guid DocumentId { get; private set; }
    public int AttemptNumber { get; private set; }
    public StatutorySubmissionOutcome Outcome { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }
    public string RequestSha256 { get; private set; } = string.Empty;
    public int? HttpStatus { get; private set; }
    public string? ExternalReference { get; private set; }
    public string ResponseJson { get; private set; } = "{}";
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    private static string? Optional(string? value, int max)
    {
        string? result = value?.Trim();
        if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

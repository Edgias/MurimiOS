using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;

public sealed class LivestockExchangeAttempt : TenantEntity
{
    private LivestockExchangeAttempt() { }

    public LivestockExchangeAttempt(Guid id, Guid submissionId, int attemptNumber,
        LivestockExchangeAttemptOutcome outcome, DateTimeOffset startedAt, DateTimeOffset completedAt,
        string requestSha256, int? httpStatus, string? externalReference, string responseJson,
        string? errorCode, string? errorMessage) : base(id)
    {
        if (submissionId == Guid.Empty || attemptNumber <= 0 || !Enum.IsDefined(outcome) || completedAt < startedAt)
            throw new ArgumentException("A valid submission, attempt number, outcome, and time range are required.");
        if (httpStatus is < 100 or > 599) throw new ArgumentOutOfRangeException(nameof(httpStatus));
        SubmissionId = submissionId;
        AttemptNumber = attemptNumber;
        Outcome = outcome;
        StartedAt = startedAt.ToUniversalTime();
        CompletedAt = completedAt.ToUniversalTime();
        RequestSha256 = Hash(requestSha256);
        HttpStatus = httpStatus;
        ExternalReference = Optional(externalReference, 160);
        ResponseJson = Json(responseJson);
        ErrorCode = Optional(errorCode, 80);
        ErrorMessage = Optional(errorMessage, 2_000);
    }

    public Guid SubmissionId { get; private set; }
    public int AttemptNumber { get; private set; }
    public LivestockExchangeAttemptOutcome Outcome { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }
    public string RequestSha256 { get; private set; } = string.Empty;
    public int? HttpStatus { get; private set; }
    public string? ExternalReference { get; private set; }
    public string ResponseJson { get; private set; } = "{}";
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x)))
            throw new ArgumentException("A SHA-256 request hash is required.", nameof(value));
        return result;
    }
    private static string Json(string? value)
    {
        string result = string.IsNullOrWhiteSpace(value) ? "{}" : value.Trim();
        if (result.Length > 128_000) throw new ArgumentException("Response JSON is too large.");
        using JsonDocument _ = JsonDocument.Parse(result);
        return result;
    }
    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

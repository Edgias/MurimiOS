using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;

public sealed class IngestionAttempt : TenantEntity
{
    private IngestionAttempt() { }
    public IngestionAttempt(Guid id, Guid jobId, int attemptNumber, DateTimeOffset startedAt, DateTimeOffset completedAt,
        IngestionAttemptOutcome outcome, int? httpStatusCode, int requestUnits, decimal cost, string? currency,
        string? errorCode, string? errorMessage) : base(id)
    {
        if (jobId == Guid.Empty || attemptNumber <= 0 || completedAt < startedAt || !Enum.IsDefined(outcome) || requestUnits < 0 || cost < 0) throw new ArgumentException("A valid ingestion attempt is required.");
        if (httpStatusCode is < 100 or > 599) throw new ArgumentOutOfRangeException(nameof(httpStatusCode));
        if (cost > 0 && string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required when provider cost is recorded.", nameof(currency));
        JobId = jobId; AttemptNumber = attemptNumber; StartedAt = startedAt.ToUniversalTime(); CompletedAt = completedAt.ToUniversalTime(); Outcome = outcome;
        HttpStatusCode = httpStatusCode; RequestUnits = requestUnits; Cost = cost; Currency = currency is null ? null : new CurrencyCode(currency).Value;
        ErrorCode = Optional(errorCode, 80); ErrorMessage = Optional(errorMessage, 2_000);
    }
    public Guid JobId { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }
    public IngestionAttemptOutcome Outcome { get; private set; }
    public int? HttpStatusCode { get; private set; }
    public int RequestUnits { get; private set; }
    public decimal Cost { get; private set; }
    public string? Currency { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.AccountingReconciliation;

public sealed class AccountingReconciliation : TenantEntity
{
    private AccountingReconciliation() { }

    public AccountingReconciliation(Guid id, Guid batchId, Guid statementLineId, Guid? transactionId,
        AccountingMatchStatus status, decimal difference, decimal tolerance, DateTimeOffset matchedAt,
        string matchedBy, string notes) : base(id)
    {
        if (batchId == Guid.Empty || statementLineId == Guid.Empty || transactionId == Guid.Empty)
            throw new ArgumentException("A batch and statement line are required; an optional transaction must be non-empty.");
        if (!Enum.IsDefined(status) || tolerance < 0) throw new ArgumentException("A valid match status and non-negative tolerance are required.");
        if (status == AccountingMatchStatus.Matched && transactionId is null)
            throw new ArgumentException("A matched reconciliation requires an internal transaction.", nameof(transactionId));
        if (status == AccountingMatchStatus.Exception && transactionId is not null)
            throw new ArgumentException("An exception cannot claim an internal transaction.", nameof(transactionId));
        BatchId = batchId; StatementLineId = statementLineId; TransactionId = transactionId;
        Status = status; Difference = decimal.Round(difference, 2, MidpointRounding.AwayFromZero);
        Tolerance = decimal.Round(tolerance, 2, MidpointRounding.AwayFromZero);
        MatchedAt = matchedAt.ToUniversalTime(); MatchedBy = Required(matchedBy, nameof(matchedBy), 240);
        Notes = Required(notes, nameof(notes), 1_000);
    }

    public Guid BatchId { get; private set; }
    public Guid StatementLineId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public AccountingMatchStatus Status { get; private set; }
    public decimal Difference { get; private set; }
    public decimal Tolerance { get; private set; }
    public DateTimeOffset MatchedAt { get; private set; }
    public string MatchedBy { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    private static string Required(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.FinanceTransaction;

public sealed class FinanceTransaction : TenantEntity
{
    private FinanceTransaction() { }

    public FinanceTransaction(Guid id, Guid farmId, FinanceEntryType entryType, string category,
        decimal amount, string currency, DateTimeOffset occurredAt, FinanceSourceType sourceType,
        Guid? sourceId, string reference, string? counterparty, string? description,
        DateTimeOffset postedAt, string postedBy, Guid? reversalOfId = null,
        Guid? counterpartyLegalCompanyId = null, string? intercompanyReference = null) : base(id)
    {
        if (farmId == Guid.Empty || sourceId == Guid.Empty || reversalOfId == Guid.Empty || counterpartyLegalCompanyId == Guid.Empty)
            throw new ArgumentException("A farm is required and optional identifiers must be non-empty.");
        if (!Enum.IsDefined(entryType) || !Enum.IsDefined(sourceType))
            throw new ArgumentOutOfRangeException("Finance entry and source types must be valid.");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (sourceType == FinanceSourceType.Reversal && reversalOfId is null)
            throw new ArgumentException("A reversal must reference its original transaction.", nameof(reversalOfId));
        if (sourceType != FinanceSourceType.Reversal && reversalOfId is not null)
            throw new ArgumentException("Only reversal transactions can reference an original transaction.", nameof(reversalOfId));
        if ((counterpartyLegalCompanyId is null) != string.IsNullOrWhiteSpace(intercompanyReference))
            throw new ArgumentException("A counterparty legal company and intercompany reference must be supplied together.");
        FarmId = farmId;
        EntryType = entryType;
        Category = Required(category, nameof(category), 120);
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = new CurrencyCode(currency).Value;
        OccurredAt = occurredAt.ToUniversalTime();
        SourceType = sourceType;
        SourceId = sourceId;
        Reference = Required(reference, nameof(reference), 160);
        Counterparty = Optional(counterparty, 240);
        Description = Optional(description, 2_000);
        PostedAt = postedAt.ToUniversalTime();
        PostedBy = Required(postedBy, nameof(postedBy), 240);
        ReversalOfId = reversalOfId;
        CounterpartyLegalCompanyId = counterpartyLegalCompanyId;
        IntercompanyReference = Optional(intercompanyReference, 160)?.ToUpperInvariant();
    }

    public Guid FarmId { get; private set; }
    public FinanceEntryType EntryType { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public FinanceSourceType SourceType { get; private set; }
    public Guid? SourceId { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string? Counterparty { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset PostedAt { get; private set; }
    public string PostedBy { get; private set; } = string.Empty;
    public Guid? ReversalOfId { get; private set; }
    public Guid? CounterpartyLegalCompanyId { get; private set; }
    public string? IntercompanyReference { get; private set; }
    public decimal SignedAmount => (ReversalOfId is null ? 1m : -1m) * Amount;

    private static string Required(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }

    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}

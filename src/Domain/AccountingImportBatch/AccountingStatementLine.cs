using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;

public sealed class AccountingStatementLine : TenantEntity
{
    private AccountingStatementLine() { }

    public AccountingStatementLine(Guid id, Guid batchId, string externalLineId, DateTimeOffset occurredAt,
        FinanceEntryType entryType, decimal amount, string currency, string accountCode,
        string? reference, string description) : base(id)
    {
        if (batchId == Guid.Empty) throw new ArgumentException("An accounting import is required.", nameof(batchId));
        if (!Enum.IsDefined(entryType) || amount <= 0) throw new ArgumentException("A valid entry type and positive amount are required.");
        BatchId = batchId; ExternalLineId = Required(externalLineId, nameof(externalLineId), 160);
        OccurredAt = occurredAt.ToUniversalTime(); EntryType = entryType;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = new CurrencyCode(currency).Value; AccountCode = Required(accountCode, nameof(accountCode), 80);
        Reference = Optional(reference, 160); Description = Required(description, nameof(description), 1_000);
    }

    public Guid BatchId { get; private set; }
    public string ExternalLineId { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public FinanceEntryType EntryType { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string AccountCode { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private static string Required(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

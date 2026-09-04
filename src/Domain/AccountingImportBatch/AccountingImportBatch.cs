using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.AccountingImportBatch;

public sealed class AccountingImportBatch : TenantEntity
{
    private AccountingImportBatch() { }

    public AccountingImportBatch(Guid id, Guid farmId, string provider, string externalBatchId,
        DateOnly statementStart, DateOnly statementEnd, string currency, DateTimeOffset importedAt,
        string importedBy) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (statementEnd < statementStart) throw new ArgumentException("Statement end cannot precede its start.");
        FarmId = farmId; Provider = Required(provider, nameof(provider), 80);
        ExternalBatchId = Required(externalBatchId, nameof(externalBatchId), 160);
        StatementStart = statementStart; StatementEnd = statementEnd;
        Currency = new CurrencyCode(currency).Value; ImportedAt = importedAt.ToUniversalTime();
        ImportedBy = Required(importedBy, nameof(importedBy), 240); Status = AccountingImportStatus.Imported;
    }

    public Guid FarmId { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string ExternalBatchId { get; private set; } = string.Empty;
    public DateOnly StatementStart { get; private set; }
    public DateOnly StatementEnd { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTimeOffset ImportedAt { get; private set; }
    public string ImportedBy { get; private set; } = string.Empty;
    public AccountingImportStatus Status { get; private set; }
    public DateTimeOffset? ReconciledAt { get; private set; }
    public string? ReconciledBy { get; private set; }

    public void CompleteReconciliation(bool hasExceptions, DateTimeOffset at, string actor)
    {
        if (Status != AccountingImportStatus.Imported)
            throw new InvalidOperationException("This accounting import has already been reconciled.");
        Status = hasExceptions ? AccountingImportStatus.Exceptions : AccountingImportStatus.Reconciled;
        ReconciledAt = at.ToUniversalTime(); ReconciledBy = Required(actor, nameof(actor), 240);
    }

    private static string Required(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

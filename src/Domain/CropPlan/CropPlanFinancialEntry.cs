using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CropPlan;

public sealed class CropPlanFinancialEntry : TenantEntity
{
    private CropPlanFinancialEntry() { }
    public CropPlanFinancialEntry(Guid id, Guid planId, Guid? allocationId, FinancialEntryType entryType,
        string category, decimal amount, string currency, DateTimeOffset occurredAt,
        string reference, string? notes, string recordedBy) : base(id)
    {
        if (planId == Guid.Empty || allocationId == Guid.Empty) throw new ArgumentException("A plan is required and an optional allocation id must be non-empty.");
        if (!Enum.IsDefined(entryType)) throw new ArgumentOutOfRangeException(nameof(entryType));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        PlanId = planId; AllocationId = allocationId; EntryType = entryType;
        Category = Text(category, nameof(category), 120); Amount = amount;
        Currency = new CurrencyCode(currency).Value; OccurredAt = occurredAt.ToUniversalTime();
        Reference = Text(reference, nameof(reference), 160); Notes = Optional(notes, 2_000);
        RecordedBy = Text(recordedBy, nameof(recordedBy), 240);
    }
    public Guid PlanId { get; private set; }
    public Guid? AllocationId { get; private set; }
    public FinancialEntryType EntryType { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public string RecordedBy { get; private set; } = string.Empty;
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

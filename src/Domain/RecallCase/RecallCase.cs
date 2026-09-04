using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.RecallCase;

public sealed class RecallCase : TenantEntity
{
    private RecallCase() { }
    public RecallCase(Guid id, string reference, string reason, DateTimeOffset initiatedAt) : base(id)
    {
        Reference = Text(reference, nameof(reference), 120);
        Reason = Text(reason, nameof(reason), 2_000);
        InitiatedAt = initiatedAt;
        Status = RecallStatus.Open;
    }
    public string Reference { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset InitiatedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public RecallStatus Status { get; private set; }
    public void Close(DateTimeOffset at) { if (Status != RecallStatus.Open) throw new InvalidOperationException("Recall is already closed."); if (at < InitiatedAt) throw new ArgumentOutOfRangeException(nameof(at)); ClosedAt = at; Status = RecallStatus.Closed; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

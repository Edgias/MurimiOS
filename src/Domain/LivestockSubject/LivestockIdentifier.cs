using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public sealed class LivestockIdentifier : TenantEntity
{
    private LivestockIdentifier() { }
    public LivestockIdentifier(Guid id, Guid subjectId, string identifierNamespace, string value,
        DateTimeOffset validFrom, bool primary) : base(id)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("A livestock subject is required.", nameof(subjectId));
        SubjectId = subjectId; IdentifierNamespace = Text(identifierNamespace, nameof(identifierNamespace), 120);
        Value = Text(value, nameof(value), 160); ValidFrom = validFrom; IsPrimary = primary;
    }
    public Guid SubjectId { get; private set; }
    public string IdentifierNamespace { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }
    public bool IsPrimary { get; private set; }
    public void Retire(DateTimeOffset at) { if (ValidTo is not null) throw new InvalidOperationException("Identifier is already retired."); if (at < ValidFrom) throw new ArgumentOutOfRangeException(nameof(at)); ValidTo = at; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

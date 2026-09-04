using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public sealed class LivestockHealthEvent : TenantEntity
{
    private LivestockHealthEvent() { }
    public LivestockHealthEvent(Guid id, Guid subjectId, LivestockHealthEventType type, string title,
        DateTimeOffset occurredAt, string? diagnosis, string? practitioner, Guid? sourceCommandId, string? notes) : base(id)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("A livestock subject is required.", nameof(subjectId));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (sourceCommandId == Guid.Empty) throw new ArgumentException("A source command must be non-empty when supplied.", nameof(sourceCommandId));
        SubjectId = subjectId; Type = type; Title = Text(title, nameof(title), 200); OccurredAt = occurredAt;
        Diagnosis = Optional(diagnosis, 500); Practitioner = Optional(practitioner, 200);
        SourceCommandId = sourceCommandId; Notes = Optional(notes, 2_000);
    }
    public Guid SubjectId { get; private set; }
    public LivestockHealthEventType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Practitioner { get; private set; }
    public Guid? SourceCommandId { get; private set; }
    public string? Notes { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

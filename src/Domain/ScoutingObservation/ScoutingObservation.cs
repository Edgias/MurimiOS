using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.ScoutingObservation;

public sealed class ScoutingObservation : TenantEntity
{
    private ScoutingObservation() { }
    public ScoutingObservation(Guid id, Guid farmId, Guid fieldId, Guid boundaryVersionId, Guid? cropZoneId,
        ScoutingCategory category, InsightSeverity severity, string? issueCode, string title, string notes,
        DateTimeOffset observedAt, DateTimeOffset recordedAt, string recordedBy, Point? location,
        Guid? evidenceId = null, Guid? sourceCommandId = null) : base(id)
    {
        if (new[] { farmId, fieldId, boundaryVersionId }.Any(x => x == Guid.Empty) || cropZoneId == Guid.Empty || evidenceId == Guid.Empty || sourceCommandId == Guid.Empty)
            throw new ArgumentException("Required identifiers must be non-empty.");
        if (!Enum.IsDefined(category) || !Enum.IsDefined(severity)) throw new ArgumentOutOfRangeException(nameof(category));
        if (location is not null) { if (location.IsEmpty || !location.IsValid || location.X is < -180 or > 180 || location.Y is < -90 or > 90) throw new ArgumentException("A valid WGS84 scouting point is required.", nameof(location)); location.SRID = 4326; }
        FarmId = farmId; FieldId = fieldId; BoundaryVersionId = boundaryVersionId; CropZoneId = cropZoneId;
        Category = category; Severity = severity; IssueCode = Optional(issueCode, 80)?.ToLowerInvariant();
        Title = Text(title, nameof(title), 200); Notes = Text(notes, nameof(notes), 4_000);
        ObservedAt = observedAt.ToUniversalTime(); RecordedAt = recordedAt.ToUniversalTime(); RecordedBy = Text(recordedBy, nameof(recordedBy), 240);
        Location = location; EvidenceId = evidenceId; SourceCommandId = sourceCommandId;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public ScoutingCategory Category { get; private set; }
    public InsightSeverity Severity { get; private set; }
    public string? IssueCode { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string RecordedBy { get; private set; } = string.Empty;
    public Point? Location { get; private set; }
    public Guid? EvidenceId { get; private set; }
    public Guid? SourceCommandId { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}

using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;

public sealed class FieldInsightAlert : TenantEntity
{
    private FieldInsightAlert() { }
    public FieldInsightAlert(Guid id, Guid farmId, Guid fieldId, Guid boundaryVersionId, string alertKey,
        string sourceType, Guid sourceId, InsightSeverity severity, string title, string message,
        DateTimeOffset detectedAt, DateTimeOffset observedAt, DateTimeOffset? expiresAt) : base(id)
    {
        if (new[] { farmId, fieldId, boundaryVersionId, sourceId }.Any(x => x == Guid.Empty) || expiresAt <= detectedAt || !Enum.IsDefined(severity)) throw new ArgumentException("Alert identity, severity, or expiry is invalid.");
        FarmId = farmId; FieldId = fieldId; BoundaryVersionId = boundaryVersionId; AlertKey = Text(alertKey, nameof(alertKey), 200).ToLowerInvariant();
        SourceType = Text(sourceType, nameof(sourceType), 100); SourceId = sourceId; Severity = severity;
        Title = Text(title, nameof(title), 200); Message = Text(message, nameof(message), 2_000);
        DetectedAt = detectedAt.ToUniversalTime(); ObservedAt = observedAt.ToUniversalTime(); ExpiresAt = expiresAt?.ToUniversalTime(); Status = InsightAlertStatus.Open;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public string AlertKey { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }
    public InsightSeverity Severity { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public DateTimeOffset DetectedAt { get; private set; }
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public InsightAlertStatus Status { get; private set; }
    public DateTimeOffset? AcknowledgedAt { get; private set; }
    public string? AcknowledgedBy { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public string? Resolution { get; private set; }
    public DateTimeOffset? DismissedAt { get; private set; }
    public string? DismissedBy { get; private set; }
    public string? DismissalReason { get; private set; }
    public Guid? FollowUpWorkOrderId { get; private set; }
    public void Acknowledge(DateTimeOffset at, string actor) { if (Status != InsightAlertStatus.Open) throw new InvalidOperationException("Only an open alert can be acknowledged."); Status = InsightAlertStatus.Acknowledged; AcknowledgedAt = at.ToUniversalTime(); AcknowledgedBy = Text(actor, nameof(actor), 240); }
    public void Resolve(DateTimeOffset at, string actor, string resolution) { if (Status is not (InsightAlertStatus.Open or InsightAlertStatus.Acknowledged)) throw new InvalidOperationException("Only an open or acknowledged alert can be resolved."); Status = InsightAlertStatus.Resolved; ResolvedAt = at.ToUniversalTime(); ResolvedBy = Text(actor, nameof(actor), 240); Resolution = Text(resolution, nameof(resolution), 2_000); }
    public void Dismiss(DateTimeOffset at, string actor, string reason) { if (Status is not (InsightAlertStatus.Open or InsightAlertStatus.Acknowledged)) throw new InvalidOperationException("Only an open or acknowledged alert can be dismissed."); Status = InsightAlertStatus.Dismissed; DismissedAt = at.ToUniversalTime(); DismissedBy = Text(actor, nameof(actor), 240); DismissalReason = Text(reason, nameof(reason), 2_000); }
    public void LinkWorkOrder(Guid workOrderId) { if (workOrderId == Guid.Empty || FollowUpWorkOrderId is not null) throw new InvalidOperationException("A distinct follow-up work order can be linked once."); FollowUpWorkOrderId = workOrderId; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

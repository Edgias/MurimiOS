using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.NotificationRecord;

public enum NotificationSeverity
{
    Information = 1,
    Warning = 2,
    Critical = 3
}

public sealed class NotificationRecord : TenantEntity
{
    private NotificationRecord() { }

    public NotificationRecord(
        Guid id,
        Guid recipientMembershipId,
        string category,
        string title,
        string body,
        NotificationSeverity severity,
        string? resourceType = null,
        Guid? resourceId = null,
        string? actionUrl = null) : base(id)
    {
        if (recipientMembershipId == Guid.Empty) throw new ArgumentException("A recipient membership is required.", nameof(recipientMembershipId));
        if (!Enum.IsDefined(severity)) throw new ArgumentOutOfRangeException(nameof(severity));
        if (resourceId is not null && string.IsNullOrWhiteSpace(resourceType))
            throw new ArgumentException("A resource type is required when a resource id is supplied.", nameof(resourceType));
        if (!string.IsNullOrWhiteSpace(actionUrl) && (!actionUrl.StartsWith('/') || actionUrl.StartsWith("//")))
            throw new ArgumentException("Notification action URLs must be application-relative.", nameof(actionUrl));

        RecipientMembershipId = recipientMembershipId;
        Category = Require(category, nameof(category), 120);
        Title = Require(title, nameof(title), 200);
        Body = Require(body, nameof(body), 2_000);
        Severity = severity;
        ResourceType = Optional(resourceType, 120);
        ResourceId = resourceId;
        ActionUrl = Optional(actionUrl, 500);
    }

    public Guid RecipientMembershipId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationSeverity Severity { get; private set; }
    public string? ResourceType { get; private set; }
    public Guid? ResourceId { get; private set; }
    public string? ActionUrl { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    public void MarkRead(DateTimeOffset readAt)
    {
        if (ReadAt is null) ReadAt = readAt.ToUniversalTime();
    }

    private static string Require(string value, string parameter, int maximumLength)
    {
        string normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is 0 || normalized.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return normalized;
    }

    private static string? Optional(string? value, int maximumLength)
    {
        string? normalized = value?.Trim();
        if (string.IsNullOrEmpty(normalized)) return null;
        if (normalized.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.");
        return normalized;
    }
}

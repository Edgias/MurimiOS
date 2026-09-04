using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.NotificationPreference;

public sealed class NotificationPreference : TenantEntity
{
    private NotificationPreference() { }

    public NotificationPreference(
        Guid id,
        Guid membershipId,
        string category,
        bool inAppEnabled,
        bool emailEnabled,
        bool smsEnabled) : base(id)
    {
        if (membershipId == Guid.Empty) throw new ArgumentException("A membership is required.", nameof(membershipId));
        MembershipId = membershipId;
        Category = NormalizeCategory(category);
        SetChannels(inAppEnabled, emailEnabled, smsEnabled);
    }

    public Guid MembershipId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public bool InAppEnabled { get; private set; }
    public bool EmailEnabled { get; private set; }
    public bool SmsEnabled { get; private set; }

    public void SetChannels(bool inAppEnabled, bool emailEnabled, bool smsEnabled)
    {
        if (!inAppEnabled && !emailEnabled && !smsEnabled)
            throw new ArgumentException("At least one notification channel must remain enabled.");
        InAppEnabled = inAppEnabled;
        EmailEnabled = emailEnabled;
        SmsEnabled = smsEnabled;
    }

    private static string NormalizeCategory(string value)
    {
        string normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (normalized.Length is 0 or > 120) throw new ArgumentException("A category of at most 120 characters is required.", nameof(value));
        return normalized;
    }
}

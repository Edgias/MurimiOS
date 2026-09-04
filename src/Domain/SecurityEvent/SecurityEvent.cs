using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SecurityEvent;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class SecurityEvent : TenantEntity
{
    private SecurityEvent() { }
    public SecurityEvent(Guid id, string eventType, SecurityEventOutcome outcome, string? subject, Guid? sessionId,
        Guid? deviceId, DateTimeOffset occurredAt, string correlationId, string? remoteAddressHash,
        string? userAgentHash, string detailsJson) : base(id)
    {
        if (!Enum.IsDefined(outcome) || sessionId == Guid.Empty || deviceId == Guid.Empty) throw new ArgumentException("Security event context is invalid.");
        EventType = ApiRateLimitPolicy.Text(eventType, 120); Outcome = outcome; Subject = Optional(subject, 240);
        SessionId = sessionId; DeviceId = deviceId; OccurredAt = occurredAt.ToUniversalTime();
        CorrelationId = ApiRateLimitPolicy.Text(correlationId, 160); RemoteAddressHash = OptionalHash(remoteAddressHash);
        UserAgentHash = OptionalHash(userAgentHash); DetailsJson = ApiRateLimitPolicy.Text(detailsJson, 20_000);
    }
    public string EventType { get; private set; } = "";
    public SecurityEventOutcome Outcome { get; private set; }
    public string? Subject { get; private set; }
    public Guid? SessionId { get; private set; }
    public Guid? DeviceId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string CorrelationId { get; private set; } = "";
    public string? RemoteAddressHash { get; private set; }
    public string? UserAgentHash { get; private set; }
    public string DetailsJson { get; private set; } = "{}";
    private static string? Optional(string? value, int maximum) { string text = value?.Trim() ?? ""; if (text.Length > maximum) throw new ArgumentException($"Text cannot exceed {maximum} characters."); return text.Length == 0 ? null : text; }
    private static string? OptionalHash(string? value) => string.IsNullOrWhiteSpace(value) ? null : ApiRateLimitPolicy.Hash(value);
}

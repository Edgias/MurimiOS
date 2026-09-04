using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy;

public sealed class ApiRateLimitPolicy : TenantEntity
{
    private ApiRateLimitPolicy() { }
    public ApiRateLimitPolicy(Guid id, Guid? publicApiClientId, int requestLimit, int windowSeconds,
        int dailyQuota, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (publicApiClientId == Guid.Empty || requestLimit is < 1 or > 1_000_000 || windowSeconds is < 1 or > 86_400
            || dailyQuota < requestLimit || dailyQuota > 100_000_000)
            throw new ArgumentException("Client scope, request window, or daily quota is invalid.");
        PublicApiClientId = publicApiClientId; RequestLimit = requestLimit; WindowSeconds = windowSeconds;
        DailyQuota = dailyQuota; CreatedAt = createdAt.ToUniversalTime(); CreatedBy = Text(createdBy, 240); IsActive = true;
    }
    public Guid? PublicApiClientId { get; private set; }
    public int RequestLimit { get; private set; }
    public int WindowSeconds { get; private set; }
    public int DailyQuota { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = "";
    public bool IsActive { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Retire(DateTimeOffset at) { if (!IsActive) throw new InvalidOperationException("Policy is already retired."); IsActive = false; RetiredAt = at.ToUniversalTime(); }
    internal static string Text(string? value, int maximum) { string text = value?.Trim() ?? ""; if (text.Length is 0 || text.Length > maximum) throw new ArgumentException($"Text must contain 1-{maximum} characters."); return text; }
    internal static string Hash(string? value) { string hash = value?.Trim().ToLowerInvariant() ?? ""; if (hash.Length != 64 || hash.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 hash is required."); return hash; }
}

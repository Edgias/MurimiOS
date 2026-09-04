using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IntegrationConnection;

public sealed class IntegrationConnection : TenantEntity
{
    private IntegrationConnection() { }
    public IntegrationConnection(Guid id, string providerCode, string name, IntegrationCategory category,
        string configurationJson, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (!Enum.IsDefined(category)) throw new ArgumentOutOfRangeException(nameof(category));
        ProviderCode = Text(providerCode, nameof(providerCode), 80).ToLowerInvariant();
        Name = Text(name, nameof(name), 200); Category = category;
        ConfigurationJson = Json(configurationJson, nameof(configurationJson), 32_000);
        Status = IntegrationConnectionStatus.Draft; Health = IntegrationHealth.Unknown;
        CreatedAt = createdAt.ToUniversalTime(); CreatedBy = Text(createdBy, nameof(createdBy), 240);
    }
    public string ProviderCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public IntegrationCategory Category { get; private set; }
    public string ConfigurationJson { get; private set; } = "{}";
    public IntegrationConnectionStatus Status { get; private set; }
    public IntegrationHealth Health { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? LastSucceededAt { get; private set; }
    public DateTimeOffset? LastFailedAt { get; private set; }
    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public DateTimeOffset? RateLimitResetAt { get; private set; }
    public int? RateLimitRemaining { get; private set; }
    public DateTimeOffset? DisabledAt { get; private set; }
    public string? DisabledBy { get; private set; }

    public void Update(string name, string configurationJson)
    {
        if (Status == IntegrationConnectionStatus.Disabled) throw new InvalidOperationException("A disabled connection cannot be edited.");
        Name = Text(name, nameof(name), 200); ConfigurationJson = Json(configurationJson, nameof(configurationJson), 32_000);
    }
    public void Activate(DateTimeOffset at)
    {
        if (Status is not (IntegrationConnectionStatus.Draft or IntegrationConnectionStatus.Paused)) throw new InvalidOperationException("Only a draft or paused connection can be activated.");
        Status = IntegrationConnectionStatus.Active; ActivatedAt ??= at.ToUniversalTime();
    }
    public void Pause() { if (Status != IntegrationConnectionStatus.Active) throw new InvalidOperationException("Only an active connection can be paused."); Status = IntegrationConnectionStatus.Paused; }
    public void Disable(DateTimeOffset at, string actor)
    {
        if (Status == IntegrationConnectionStatus.Disabled) throw new InvalidOperationException("The connection is already disabled.");
        Status = IntegrationConnectionStatus.Disabled; DisabledAt = at.ToUniversalTime(); DisabledBy = Text(actor, nameof(actor), 240);
    }
    public void RecordSuccess(DateTimeOffset at, int? rateLimitRemaining, DateTimeOffset? rateLimitResetAt)
    {
        if (rateLimitRemaining < 0) throw new ArgumentOutOfRangeException(nameof(rateLimitRemaining));
        Health = IntegrationHealth.Healthy; LastSucceededAt = at.ToUniversalTime(); LastErrorCode = null; LastErrorMessage = null;
        RateLimitRemaining = rateLimitRemaining; RateLimitResetAt = rateLimitResetAt?.ToUniversalTime();
    }
    public void RecordFailure(DateTimeOffset at, string code, string message, bool unavailable)
    {
        Health = unavailable ? IntegrationHealth.Unavailable : IntegrationHealth.Degraded;
        LastFailedAt = at.ToUniversalTime(); LastErrorCode = Text(code, nameof(code), 80); LastErrorMessage = Text(message, nameof(message), 2_000);
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string Json(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}

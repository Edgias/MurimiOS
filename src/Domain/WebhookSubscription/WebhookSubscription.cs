using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.WebhookSubscription;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class WebhookSubscription : TenantEntity
{
    private WebhookSubscription() { }
    public WebhookSubscription(Guid id, string name, string endpoint, IEnumerable<string> eventTypes, string signingKeyHash,
        int maximumAttempts, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri) || uri.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment)) throw new ArgumentException("Webhook endpoint must be an absolute HTTPS URL without credentials or a fragment.");
        string[] events = eventTypes.Select(x => x?.Trim() ?? "").Where(x => x.Length > 0).Distinct(StringComparer.Ordinal).Order().ToArray();
        if (events.Length == 0 || events.Any(x => x.Length > 200) || maximumAttempts is < 1 or > 20) throw new ArgumentException("Event types and 1-20 delivery attempts are required.");
        Name = PublicApiClient.Text(name, 200); Endpoint = uri.AbsoluteUri; EventTypes = events; SigningKeyHash = PublicApiClient.Hash(signingKeyHash);
        MaximumAttempts = maximumAttempts; RegisteredAt = createdAt.ToUniversalTime(); CreatedBy = PublicApiClient.Text(createdBy, 240); Status = WebhookStatus.Active;
    }
    public string Name { get; private set; } = ""; public string Endpoint { get; private set; } = ""; public string[] EventTypes { get; private set; } = [];
    public string SigningKeyHash { get; private set; } = ""; public int SecretVersion { get; private set; } = 1; public int MaximumAttempts { get; private set; }
    public WebhookStatus Status { get; private set; } public DateTimeOffset RegisteredAt { get; private set; } public string CreatedBy { get; private set; } = "";
    public DateTimeOffset? DisabledAt { get; private set; }
    public void Suspend() { if (Status != WebhookStatus.Active) throw new InvalidOperationException("Only an active webhook can be suspended."); Status = WebhookStatus.Suspended; }
    public void Activate() { if (Status != WebhookStatus.Suspended) throw new InvalidOperationException("Only a suspended webhook can be activated."); Status = WebhookStatus.Active; }
    public void Disable(DateTimeOffset at) { if (Status == WebhookStatus.Disabled) throw new InvalidOperationException("Webhook is already disabled."); Status = WebhookStatus.Disabled; DisabledAt = at.ToUniversalTime(); }
    public void RotateSecret(string signingKeyHash) { if (Status == WebhookStatus.Disabled) throw new InvalidOperationException("A disabled webhook cannot rotate secrets."); SigningKeyHash = PublicApiClient.Hash(signingKeyHash); SecretVersion++; }
}

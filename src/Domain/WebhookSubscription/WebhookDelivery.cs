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

public sealed class WebhookDelivery : TenantEntity
{
    private WebhookDelivery() { }
    public WebhookDelivery(Guid id, Guid subscriptionId, Guid outboxMessageId, string eventType, string payload, DateTimeOffset occurredAt, int maximumAttempts) : base(id)
    { if (subscriptionId == Guid.Empty || outboxMessageId == Guid.Empty || maximumAttempts is < 1 or > 20) throw new ArgumentException("Delivery identifiers and attempts are required."); SubscriptionId = subscriptionId; OutboxMessageId = outboxMessageId; EventType = PublicApiClient.Text(eventType, 200); Payload = PublicApiClient.Text(payload, 1_000_000); OccurredAt = occurredAt.ToUniversalTime(); AvailableAt = OccurredAt; MaximumAttempts = maximumAttempts; Status = WebhookDeliveryStatus.Pending; }
    public Guid SubscriptionId { get; private set; } public Guid OutboxMessageId { get; private set; } public string EventType { get; private set; } = ""; public string Payload { get; private set; } = "{}";
    public DateTimeOffset OccurredAt { get; private set; } public DateTimeOffset AvailableAt { get; private set; } public WebhookDeliveryStatus Status { get; private set; }
    public int Attempts { get; private set; } public int MaximumAttempts { get; private set; } public Guid? LeaseId { get; private set; } public DateTimeOffset? LeasedUntil { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; } public int? ResponseStatus { get; private set; } public string? ResponseBodyExcerpt { get; private set; } public string? LastError { get; private set; }
    public void Claim(Guid leaseId, DateTimeOffset at, TimeSpan duration) { if (leaseId == Guid.Empty || duration <= TimeSpan.Zero || Status is not (WebhookDeliveryStatus.Pending or WebhookDeliveryStatus.RetryScheduled) || AvailableAt > at) throw new InvalidOperationException("Delivery is not claimable."); LeaseId = leaseId; LeasedUntil = at.ToUniversalTime().Add(duration); Attempts++; Status = WebhookDeliveryStatus.Running; }
    public void Succeed(Guid leaseId, DateTimeOffset at, int responseStatus, string? excerpt) { Lease(leaseId, at); if (responseStatus is < 200 or > 299) throw new ArgumentOutOfRangeException(nameof(responseStatus)); Status = WebhookDeliveryStatus.Delivered; DeliveredAt = at.ToUniversalTime(); ResponseStatus = responseStatus; ResponseBodyExcerpt = Optional(excerpt); LeaseId = null; LeasedUntil = null; LastError = null; }
    public void Fail(Guid leaseId, DateTimeOffset at, DateTimeOffset retryAt, int? responseStatus, string error, string? excerpt) { Lease(leaseId, at); LastError = PublicApiClient.Text(error, 2000); ResponseStatus = responseStatus; ResponseBodyExcerpt = Optional(excerpt); LeaseId = null; LeasedUntil = null; if (Attempts >= MaximumAttempts) Status = WebhookDeliveryStatus.DeadLettered; else { Status = WebhookDeliveryStatus.RetryScheduled; AvailableAt = retryAt.ToUniversalTime(); } }
    public void Replay(DateTimeOffset at) { if (Status != WebhookDeliveryStatus.DeadLettered) throw new InvalidOperationException("Only a dead-lettered delivery can be replayed."); Status = WebhookDeliveryStatus.RetryScheduled; AvailableAt = at.ToUniversalTime(); Attempts = 0; LastError = null; }
    private void Lease(Guid id, DateTimeOffset at) { if (Status != WebhookDeliveryStatus.Running || LeaseId != id || LeasedUntil < at) throw new InvalidOperationException("Delivery lease is invalid or expired."); }
    private static string? Optional(string? value) { string? text = value?.Trim(); return string.IsNullOrEmpty(text) ? null : text[..Math.Min(2000, text.Length)]; }
}

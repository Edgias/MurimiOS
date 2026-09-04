using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;

using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;
using WebhookDeliveryStatus = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDeliveryStatus;

public sealed class GetGroupWebhooksSubscriptionIdGuidDeliveriesHandler(IPlatformDbContext db) : IRequestHandler<GetGroupWebhooksSubscriptionIdGuidDeliveriesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetGroupWebhooksSubscriptionIdGuidDeliveriesQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.subscriptionId, message.status, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid subscriptionId, WebhookDeliveryStatus? status, IPlatformDbContext db, CancellationToken ct) { IQueryable<WebhookDelivery> query = db.WebhookDeliveries.AsNoTracking().Where(x => x.SubscriptionId == subscriptionId); if (status.HasValue) query = query.Where(x => x.Status == status); return UseCaseResults.Ok(await query.OrderByDescending(x => x.OccurredAt).Take(1000).ToArrayAsync(ct)); }

}

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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PostGroupWebhookDeliveriesDeliveryIdGuidReplay;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;

using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;

public sealed class PostGroupWebhookDeliveriesDeliveryIdGuidReplayHandler(IPlatformDbContext db) : IRequestHandler<PostGroupWebhookDeliveriesDeliveryIdGuidReplayCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostGroupWebhookDeliveriesDeliveryIdGuidReplayCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.deliveryId, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid deliveryId, IPlatformDbContext db, CancellationToken ct) { WebhookDelivery? delivery = await db.WebhookDeliveries.SingleOrDefaultAsync(x => x.Id == deliveryId, ct); if (delivery is null) return UseCaseResults.NotFound(); try { delivery.Replay(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(delivery); } catch (InvalidOperationException e) { return Conflict("webhook_replay_invalid", e.Message); } }
    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

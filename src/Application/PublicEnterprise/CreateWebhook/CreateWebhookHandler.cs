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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateWebhook;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;

public sealed class CreateWebhookHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<CreateWebhookCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateWebhookCommand message, CancellationToken cancellationToken) => CreateWebhookAsync(message.request, db, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateWebhookAsync(CreateWebhookRequest request, IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    {
        (string secret, _, string hash) = Secret("mro_whsec");
        try { var webhook = new WebhookSubscription(Id(request.Id), request.Name, request.Endpoint, request.EventTypes, hash, request.MaximumAttempts, DateTimeOffset.UtcNow, actor.Subject); db.WebhookSubscriptions.Add(webhook); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/public-api/webhooks/{webhook.Id}", new { webhook, signingSecret = secret, signatureKeyDerivation = "SHA-256(signingSecret)" }); }
        catch (ArgumentException e) { return Bad("webhook_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static (string Secret, string Prefix, string Hash) Secret(string prefix) { string raw = $"{prefix}_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}"; return (raw, raw[..Math.Min(16, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}

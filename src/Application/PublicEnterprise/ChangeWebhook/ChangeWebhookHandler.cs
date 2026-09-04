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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ChangeWebhook;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;

public sealed class ChangeWebhookHandler(IPlatformDbContext db) : IRequestHandler<ChangeWebhookCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeWebhookCommand message, CancellationToken cancellationToken) => ChangeWebhookAsync(message.subscriptionId, message.command, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeWebhookAsync(Guid subscriptionId, string command, IPlatformDbContext db, CancellationToken ct)
    {
        WebhookSubscription? webhook = await db.WebhookSubscriptions.SingleOrDefaultAsync(x => x.Id == subscriptionId, ct); if (webhook is null) return UseCaseResults.NotFound();
        try { if (command == "activate") webhook.Activate(); else if (command == "suspend") webhook.Suspend(); else if (command == "disable") webhook.Disable(DateTimeOffset.UtcNow); else { (string secret, _, string hash) = Secret("mro_whsec"); webhook.RotateSecret(hash); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { webhook, signingSecret = secret, signatureKeyDerivation = "SHA-256(signingSecret)" }); } await db.SaveChangesAsync(ct); return UseCaseResults.Ok(webhook); }
        catch (InvalidOperationException e) { return Conflict("webhook_transition_invalid", e.Message); }
    }

    private static (string Secret, string Prefix, string Hash) Secret(string prefix) { string raw = $"{prefix}_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}"; return (raw, raw[..Math.Min(16, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.IssueTransparencyToken;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using DisclosurePolicyStatus = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.DisclosurePolicyStatus;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

public sealed class IssueTransparencyTokenHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<IssueTransparencyTokenCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IssueTransparencyTokenCommand message, CancellationToken cancellationToken) => IssueTransparencyTokenAsync(message.policyId, message.request, db, actor, tenant, cancellationToken);

    private static async Task<UseCaseResult> IssueTransparencyTokenAsync(Guid policyId, IssueTransparencyTokenRequest request,
        IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, CancellationToken ct)
    {
        RetailDisclosurePolicy? policy = await db.RetailDisclosurePolicies.SingleOrDefaultAsync(x => x.Id == policyId && x.Status == DisclosurePolicyStatus.Published, ct); if (policy is null) return UseCaseResults.NotFound();
        if (policy.DataGrantId is null)
        {
            Guid? farmId = await (from lot in db.HarvestLots where lot.Id == request.HarvestLotId join zone in db.CropZones on lot.CropZoneId equals zone.Id join season in db.GrowingSeasons on zone.SeasonId equals season.Id select (Guid?)season.FarmId).SingleOrDefaultAsync(ct);
            if (farmId is null || policy.FarmIds.Length > 0 && !policy.FarmIds.Contains(farmId.Value)) return Bad("harvest_scope_invalid", "Harvest lot is outside the disclosure policy.");
        }
        else
        {
            bool granted = await db.Set<GrantedHarvestLotProjection>().FromSqlInterpolated($"SELECT * FROM supply.read_granted_harvest_lots({policy.DataGrantId.Value})").AnyAsync(x => x.HarvestLotId == request.HarvestLotId, ct);
            if (!granted) return Bad("grant_harvest_invalid", "The grant does not disclose this harvest lot.");
        }
        DateTimeOffset now = DateTimeOffset.UtcNow; (string raw, string prefix, string hash) = Secret("mro_trace");
        try { var token = new TransparencyToken(Id(request.Id), policyId, TraceObjectType.HarvestLot, request.HarvestLotId, prefix, hash, now, request.ExpiresAt, actor.Subject); db.TransparencyTokens.Add(token); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "RetailTransparency.TokenIssued", JsonSerializer.Serialize(new { token.Id, token.PolicyId, token.ObjectId, token.ExpiresAt }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/retail-transparency/tokens/{token.Id}", new { token, publicToken = raw, resolverPath = $"/api/public/v1/transparency/{raw}" }); }
        catch (ArgumentException e) { return Bad("transparency_token_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static (string Secret, string Prefix, string Hash) Secret(string prefix) { string raw = $"{prefix}_{EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32))}"; return (raw, raw[..Math.Min(16, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}

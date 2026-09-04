using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ResolveTransparency;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TransparencyAccessLog = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyAccessLog;

public sealed class ResolveTransparencyHandler(UseCaseContext context, IPlatformDbContext db, IMutableTenantContext tenant) : IRequestHandler<ResolveTransparencyQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ResolveTransparencyQuery message, CancellationToken cancellationToken) => ResolveTransparencyAsync(message.token, context, db, tenant, cancellationToken);

    private static async Task<UseCaseResult> ResolveTransparencyAsync(string token, UseCaseContext context, IPlatformDbContext db, IMutableTenantContext tenant, CancellationToken ct)
    {
        string hash = Sha(token); TransparencyIdentity? identity = await ResolveTransparencyIdentityAsync(hash, db, ct); if (identity is null) return UseCaseResults.NotFound(); tenant.Resolve(identity.TenantId);
        Dictionary<string, object?> response = new() { ["brand"] = identity.PublicBrandName, ["locale"] = identity.Locale, ["verifiedAt"] = DateTimeOffset.UtcNow };
        if (identity.DataGrantId is null)
        {
            var row = await (from lot in db.HarvestLots.AsNoTracking() where lot.Id == identity.ObjectId join zone in db.CropZones.AsNoTracking() on lot.CropZoneId equals zone.Id join season in db.GrowingSeasons.AsNoTracking() on zone.SeasonId equals season.Id join farm in db.Farms.AsNoTracking() on season.FarmId equals farm.Id select new { lot, zone, season, farm }).SingleOrDefaultAsync(ct);
            if (row is null || identity.FarmIds.Length > 0 && !identity.FarmIds.Contains(row.farm.Id)) return UseCaseResults.NotFound();
            Add(response, identity.Attributes, "origin", new { farm = row.farm.Name, row.farm.CountryCode }); Add(response, identity.Attributes, "crop", row.zone.Crop); Add(response, identity.Attributes, "variety", row.zone.Variety); Add(response, identity.Attributes, "lotCode", row.lot.LotCode); Add(response, identity.Attributes, "harvestedAt", row.lot.HarvestedAt); Add(response, identity.Attributes, "quantity", new { row.lot.Quantity, row.lot.Unit });
            if (identity.Attributes.Contains("quality")) response["quality"] = await db.QualityAssessments.AsNoTracking().Where(x => x.ObjectType == TraceObjectType.HarvestLot && x.ObjectId == identity.ObjectId).OrderByDescending(x => x.AssessedAt).Select(x => new { x.Metric, x.NumericResult, x.TextResult, x.Unit, x.Passed, x.AssessedAt }).ToArrayAsync(ct);
            if (identity.Attributes.Contains("compliance")) response["compliance"] = await db.ComplianceAssessments.AsNoTracking().Where(x => x.FarmId == row.farm.Id && x.Status == ComplianceAssessmentStatus.Completed).OrderByDescending(x => x.CompletedAt).Select(x => new { x.ScorePercent, x.Conforms, x.CompletedAt }).FirstOrDefaultAsync(ct);
            if (identity.Attributes.Contains("lineage")) response["lineage"] = TraceGraph(identity.ObjectId, await db.TraceLinks.AsNoTracking().ToArrayAsync(ct));
        }
        else
        {
            JsonElement? presentation = await ReadGrantedPresentationAsync(hash, db, ct); if (presentation is null) return UseCaseResults.NotFound();
            foreach (JsonProperty property in presentation.Value.EnumerateObject()) response[property.Name] = property.Value.Clone();
            if (identity.Attributes.Contains("lineage")) response["lineage"] = await db.Set<GrantedTraceLinkProjection>().FromSqlInterpolated($"SELECT * FROM supply.read_granted_trace_links({identity.DataGrantId.Value}, {identity.ObjectId})").AsNoTracking().ToArrayAsync(ct);
        }
        db.TransparencyAccessLogs.Add(new TransparencyAccessLog(Guid.NewGuid(), identity.TokenId, DateTimeOffset.UtcNow, "Allowed", context.User.CorrelationId, RemoteHash(context), HeaderHash(context.User.Header("User-Agent")))); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(response);
    }


    private static async Task<TransparencyIdentity?> ResolveTransparencyIdentityAsync(string hash, IPlatformDbContext db, CancellationToken ct)
    {
        TransparencyIdentity? result = null; await db.Database.OpenConnectionAsync(ct); try { await using var command = db.Database.GetDbConnection().CreateCommand(); command.CommandText = "select tenant_id,token_id,policy_id,object_id,data_grant_id,public_brand_name,locale,attributes,farm_ids from retail.resolve_transparency_token(@hash)"; var parameter = command.CreateParameter(); parameter.ParameterName = "hash"; parameter.Value = hash; command.Parameters.Add(parameter); await using var reader = await command.ExecuteReaderAsync(ct); if (await reader.ReadAsync(ct)) result = new(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2), reader.GetGuid(3), reader.IsDBNull(4) ? null : reader.GetGuid(4), reader.GetString(5), reader.GetString(6), reader.GetFieldValue<string[]>(7), reader.GetFieldValue<Guid[]>(8)); } finally { await db.Database.CloseConnectionAsync(); } return result;
    }


    private static async Task<JsonElement?> ReadGrantedPresentationAsync(string hash, IPlatformDbContext db, CancellationToken ct)
    {
        string? json; await db.Database.OpenConnectionAsync(ct); try { await using var command = db.Database.GetDbConnection().CreateCommand(); command.CommandText = "select retail.resolve_granted_transparency_presentation(@hash)::text"; var parameter = command.CreateParameter(); parameter.ParameterName = "hash"; parameter.Value = hash; command.Parameters.Add(parameter); json = await command.ExecuteScalarAsync(ct) as string; } finally { await db.Database.CloseConnectionAsync(); }
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<JsonElement>(json);
    }

    private static object TraceGraph(Guid root, IReadOnlyList<TraceLink> links) { var nodes = new HashSet<(TraceObjectType, Guid)> { (TraceObjectType.HarvestLot, root) }; var selected = new List<TraceLink>(); for (int depth = 0; depth < 25; depth++) { TraceLink[] next = links.Where(x => nodes.Contains((x.FromType, x.FromId)) && !nodes.Contains((x.ToType, x.ToId))).ToArray(); if (next.Length == 0) break; selected.AddRange(next); foreach (TraceLink x in next) nodes.Add((x.ToType, x.ToId)); } return new { root = new { objectType = TraceObjectType.HarvestLot, objectId = root }, nodes = nodes.Select(x => new { objectType = x.Item1, objectId = x.Item2 }), links = selected }; }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static string? RemoteHash(UseCaseContext context) => context.User.RemoteAddress is { } address ? Sha(address.ToString()) : null;

    private static string? HeaderHash(string? value) => string.IsNullOrWhiteSpace(value) ? null : Sha(value);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private sealed record TransparencyIdentity(Guid TenantId, Guid TokenId, Guid PolicyId, Guid ObjectId, Guid? DataGrantId, string PublicBrandName, string Locale, string[] Attributes, Guid[] FarmIds);

}

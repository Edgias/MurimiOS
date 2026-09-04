using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.PublicApiClient;

public sealed class PublicApiClient : TenantEntity
{
    private PublicApiClient() { }
    public PublicApiClient(Guid id, string name, IEnumerable<string> scopes, IEnumerable<Guid> farmIds,
        DateTimeOffset createdAt, string createdBy) : base(id)
    {
        Name = Text(name, 200); Scopes = Set(scopes, PublicApiScopes.All, true); FarmIds = Ids(farmIds);
        CreatedBy = Text(createdBy, 240); RegisteredAt = createdAt.ToUniversalTime(); Status = PublicApiClientStatus.Active;
    }
    public string Name { get; private set; } = string.Empty;
    public string[] Scopes { get; private set; } = [];
    public Guid[] FarmIds { get; private set; } = [];
    public PublicApiClientStatus Status { get; private set; }
    public DateTimeOffset RegisteredAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset? LastUsedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public void Suspend() { if (Status != PublicApiClientStatus.Active) throw new InvalidOperationException("Only an active client can be suspended."); Status = PublicApiClientStatus.Suspended; }
    public void Activate() { if (Status != PublicApiClientStatus.Suspended) throw new InvalidOperationException("Only a suspended client can be activated."); Status = PublicApiClientStatus.Active; }
    public void Revoke(DateTimeOffset at, string actor) { if (Status == PublicApiClientStatus.Revoked) throw new InvalidOperationException("Client is already revoked."); Status = PublicApiClientStatus.Revoked; RevokedAt = at.ToUniversalTime(); RevokedBy = Text(actor, 240); }
    public void RecordUse(DateTimeOffset at) { if (Status != PublicApiClientStatus.Active) throw new InvalidOperationException("Only an active client can be used."); LastUsedAt = at.ToUniversalTime(); }
    internal static string Text(string? value, int max) { string text = value?.Trim() ?? string.Empty; if (text.Length == 0 || text.Length > max) throw new ArgumentException($"Text must contain 1-{max} characters."); return text; }
    internal static string Hash(string? value) { string hash = value?.Trim().ToLowerInvariant() ?? string.Empty; if (hash.Length != 64 || hash.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 hash is required."); return hash; }
    internal static string[] Set(IEnumerable<string> values, IReadOnlySet<string> allowed, bool required) { string[] set = values.Select(x => x?.Trim() ?? "").Where(x => x.Length > 0).Distinct(StringComparer.Ordinal).Order().ToArray(); if (required && set.Length == 0 || set.Any(x => !allowed.Contains(x))) throw new ArgumentException("Unsupported or missing values."); return set; }
    internal static Guid[] Ids(IEnumerable<Guid> values) { Guid[] ids = values.Distinct().Order().ToArray(); if (ids.Any(x => x == Guid.Empty)) throw new ArgumentException("Identifiers must be non-empty."); return ids; }
}

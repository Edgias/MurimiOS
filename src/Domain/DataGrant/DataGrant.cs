

namespace EdgIQ.MurimiOS.Domain.DataGrant;

public sealed class DataGrant
{
    private DataGrant() { }
    public DataGrant(Guid id, Guid grantSeriesId, Guid relationshipId, Guid producerTenantId, Guid recipientTenantId,
        string name, string purpose, IEnumerable<string> entityCategories, IEnumerable<string> attributes,
        IEnumerable<Guid> farmIds, IEnumerable<Guid>? seasonIds, IEnumerable<string>? commodities,
        DateTimeOffset validFrom, DateTimeOffset? validTo, bool downstreamDisclosureAllowed,
        int grantVersion, Guid? previousGrantId, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty || grantSeriesId == Guid.Empty || relationshipId == Guid.Empty || producerTenantId == Guid.Empty || recipientTenantId == Guid.Empty)
            throw new ArgumentException("Grant, relationship, producer, and recipient identifiers are required.");
        if (producerTenantId == recipientTenantId) throw new ArgumentException("A data grant requires distinct tenants.");
        if (validTo <= validFrom) throw new ArgumentException("Grant expiry must follow its start.");
        if (grantVersion <= 0 || previousGrantId == Guid.Empty) throw new ArgumentException("Grant version must be positive and a previous grant id must be non-empty when supplied.");
        Id = id; GrantSeriesId = grantSeriesId; RelationshipId = relationshipId; ProducerTenantId = producerTenantId; RecipientTenantId = recipientTenantId;
        Name = Text(name, nameof(name), 200); Purpose = Text(purpose, nameof(purpose), 240);
        EntityCategories = Normalize(entityCategories, SupplyDataCategories.All, "entity category");
        Attributes = Normalize(attributes, null, "attribute"); FarmIds = NormalizeIds(farmIds, true, "farm");
        SeasonIds = NormalizeIds(seasonIds ?? [], false, "season"); Commodities = Normalize(commodities ?? [], null, "commodity", allowEmpty: true);
        ValidFrom = validFrom; ValidTo = validTo; DownstreamDisclosureAllowed = downstreamDisclosureAllowed;
        GrantVersion = grantVersion; PreviousGrantId = previousGrantId; Status = DataGrantStatus.Active;
        CreatedAtUtc = createdAt; UpdatedAtUtc = createdAt; Version = 1;
    }
    public Guid Id { get; private set; }
    public Guid GrantSeriesId { get; private set; }
    public Guid RelationshipId { get; private set; }
    public Guid ProducerTenantId { get; private set; }
    public Guid RecipientTenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Purpose { get; private set; } = string.Empty;
    public string[] EntityCategories { get; private set; } = [];
    public string[] Attributes { get; private set; } = [];
    public Guid[] FarmIds { get; private set; } = [];
    public Guid[] SeasonIds { get; private set; } = [];
    public string[] Commodities { get; private set; } = [];
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }
    public bool DownstreamDisclosureAllowed { get; private set; }
    public int GrantVersion { get; private set; }
    public Guid? PreviousGrantId { get; private set; }
    public DataGrantStatus Status { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedByTenantId { get; private set; }
    public string? RevocationReason { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public uint Version { get; private set; }

    public bool IsEffective(Guid requestingTenantId, string purpose, string category, DateTimeOffset at) =>
        requestingTenantId == RecipientTenantId && Status == DataGrantStatus.Active
        && string.Equals(Purpose, purpose?.Trim(), StringComparison.Ordinal)
        && EntityCategories.Contains(category, StringComparer.Ordinal)
        && ValidFrom <= at && (ValidTo is null || ValidTo > at);
    public void Revoke(Guid actorTenantId, DateTimeOffset at, string reason)
    {
        if (actorTenantId != ProducerTenantId) throw new InvalidOperationException("Only the producer can revoke a data grant.");
        if (Status != DataGrantStatus.Active) throw new InvalidOperationException("Grant is already revoked.");
        Status = DataGrantStatus.Revoked; RevokedAt = at; RevokedByTenantId = actorTenantId;
        RevocationReason = Text(reason, nameof(reason), 1_000); UpdatedAtUtc = at; Version++;
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string[] Normalize(IEnumerable<string> values, IReadOnlySet<string>? allowed, string label, bool allowEmpty = false)
    {
        string[] result = values.Select(x => x?.Trim() ?? string.Empty).Where(x => x.Length > 0).Distinct(StringComparer.Ordinal).Order().ToArray();
        if (!allowEmpty && result.Length == 0) throw new ArgumentException($"At least one {label} is required.");
        if (result.Any(x => x.Length > 160 || allowed is not null && !allowed.Contains(x))) throw new ArgumentException($"An unsupported {label} was supplied.");
        return result;
    }
    private static Guid[] NormalizeIds(IEnumerable<Guid> values, bool required, string label)
    {
        Guid[] result = values.Distinct().Order().ToArray();
        if (result.Any(x => x == Guid.Empty) || required && result.Length == 0) throw new ArgumentException($"Valid {label} scope identifiers are required.");
        return result;
    }
}

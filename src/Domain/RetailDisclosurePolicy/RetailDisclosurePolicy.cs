using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class RetailDisclosurePolicy : TenantEntity
{
    private RetailDisclosurePolicy() { }
    public RetailDisclosurePolicy(Guid id, Guid seriesId, string name, string publicBrandName, IEnumerable<string> attributes,
        IEnumerable<Guid> farmIds, Guid? dataGrantId, int policyVersion, Guid? previousPolicyId, string locale, DateTimeOffset createdAt, string createdBy) : base(id)
    { if (seriesId == Guid.Empty || dataGrantId == Guid.Empty || previousPolicyId == Guid.Empty || policyVersion <= 0) throw new ArgumentException("Policy lineage is invalid."); Id = id; SeriesId = seriesId; Name = PublicApiClient.Text(name, 200); PublicBrandName = PublicApiClient.Text(publicBrandName, 200); Attributes = PublicApiClient.Set(attributes, DisclosureAttributes.All, true); FarmIds = PublicApiClient.Ids(farmIds); DataGrantId = dataGrantId; PolicyVersion = policyVersion; PreviousPolicyId = previousPolicyId; Locale = PublicApiClient.Text(locale, 40); AuthoredAt = createdAt.ToUniversalTime(); AuthoredBy = PublicApiClient.Text(createdBy, 240); Status = DisclosurePolicyStatus.Draft; }
    public Guid SeriesId { get; private set; } public string Name { get; private set; } = ""; public string PublicBrandName { get; private set; } = ""; public string[] Attributes { get; private set; } = []; public Guid[] FarmIds { get; private set; } = [];
    public Guid? DataGrantId { get; private set; } public int PolicyVersion { get; private set; } public Guid? PreviousPolicyId { get; private set; } public string Locale { get; private set; } = ""; public DisclosurePolicyStatus Status { get; private set; }
    public DateTimeOffset AuthoredAt { get; private set; } public string AuthoredBy { get; private set; } = ""; public DateTimeOffset? PublishedAt { get; private set; } public DateTimeOffset? RetiredAt { get; private set; }
    public void Publish(DateTimeOffset at) { if (Status != DisclosurePolicyStatus.Draft) throw new InvalidOperationException("Only a draft policy can be published."); Status = DisclosurePolicyStatus.Published; PublishedAt = at.ToUniversalTime(); }
    public void Retire(DateTimeOffset at) { if (Status != DisclosurePolicyStatus.Published) throw new InvalidOperationException("Only a published policy can be retired."); Status = DisclosurePolicyStatus.Retired; RetiredAt = at.ToUniversalTime(); }
}

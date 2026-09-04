using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.TenantPlacement;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class TenantPlacement : TenantEntity
{
    private TenantPlacement() { }
    public TenantPlacement(Guid id, string region, string databaseKey, TenantDatabaseMode mode, string residencyCountry, bool isPrimary, DateTimeOffset effectiveFrom) : base(id)
    { Region = PublicApiClient.Text(region, 80); DatabaseKey = PublicApiClient.Text(databaseKey, 160); Mode = mode; string country = PublicApiClient.Text(residencyCountry, 2); if (country.Length != 2) throw new ArgumentException("Residency country must be a two-letter code."); ResidencyCountry = country.ToUpperInvariant(); IsPrimary = isPrimary; EffectiveFrom = effectiveFrom.ToUniversalTime(); }
    public string Region { get; private set; } = ""; public string DatabaseKey { get; private set; } = ""; public TenantDatabaseMode Mode { get; private set; } public string ResidencyCountry { get; private set; } = ""; public bool IsPrimary { get; private set; } public DateTimeOffset EffectiveFrom { get; private set; } public DateTimeOffset? EffectiveTo { get; private set; }
    public void Close(DateTimeOffset at) { if (EffectiveTo is not null || at <= EffectiveFrom) throw new InvalidOperationException("Placement cannot be closed."); EffectiveTo = at.ToUniversalTime(); IsPrimary = false; }
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;

namespace EdgIQ.MurimiOS.Domain.TraceEvent;

public enum TraceRelationshipType
{
    InputApplied = 1,
    TransformedInto = 2,
    AggregatedInto = 3,
    DeliveredAs = 4,
    ConvertedToFeed = 5
}

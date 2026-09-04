using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;

namespace EdgIQ.MurimiOS.Domain.TraceEvent;

public enum TraceEventType
{
    Object = 1,
    Aggregation = 2,
    Transformation = 3,
    Transaction = 4
}

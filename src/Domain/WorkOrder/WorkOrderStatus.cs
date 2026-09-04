using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public enum WorkOrderStatus
{
    Draft = 1,
    Planned = 2,
    Assigned = 3,
    InProgress = 4,
    Completed = 5,
    Verified = 6,
    Rejected = 7,
    Cancelled = 8,
    Superseded = 9
}

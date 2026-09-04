using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.FinanceTransaction;

public enum CostAllocationMethod
{
    Direct = 1,
    Area = 2,
    Headcount = 3,
    Equal = 4,
    Percentage = 5
}

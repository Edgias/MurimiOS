using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.FinanceTransaction;

public enum FinanceSourceType
{
    Manual = 1,
    Labour = 2,
    Machinery = 3,
    Inventory = 4,
    Accounting = 5,
    Reversal = 6,
    Planning = 7
}

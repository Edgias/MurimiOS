using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.AccountingImportBatch;

public enum AccountingImportStatus
{
    Imported = 1,
    Reconciled = 2,
    Exceptions = 3
}

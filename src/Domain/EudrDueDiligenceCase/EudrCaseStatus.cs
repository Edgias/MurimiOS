using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public enum EudrCaseStatus
{
    Draft = 1,
    Screening = 2,
    Assessed = 3,
    Ready = 4,
    Exported = 5,
    Submitted = 6,
    Cancelled = 7
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public enum EudrScreeningResult
{
    Clear = 1,
    ReviewRequired = 2,
    PotentialDeforestation = 3
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
public enum ReproductionOutcome
{
    Performed = 1, Pregnant = 2, Open = 3, Inconclusive = 4, LiveBirth = 5,
    Stillbirth = 6, MixedBirth = 7, Abortion = 8, Weaned = 9
}

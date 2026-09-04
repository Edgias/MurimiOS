using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;

public enum StatutorySubmissionOutcome
{
    Accepted = 1,
    Rejected = 2,
    RetriableFailure = 3
}

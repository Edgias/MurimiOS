using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;
public enum IngestionAttemptOutcome { Succeeded = 1, RetriableFailure = 2, PermanentFailure = 3 }

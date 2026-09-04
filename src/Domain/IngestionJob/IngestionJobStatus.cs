using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;
public enum IngestionJobStatus { Queued = 1, Running = 2, Succeeded = 3, RetryScheduled = 4, DeadLettered = 5, Cancelled = 6 }

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;
public enum IngestionJobKind { Poll = 1, Backfill = 2, Webhook = 3, Import = 4, Replay = 5 }

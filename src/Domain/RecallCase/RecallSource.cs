using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.RecallCase;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class RecallSource : TenantEntity
{
    private RecallSource() { }
    public RecallSource(Guid id, Guid recallCaseId, TraceObjectType objectType, Guid objectId) : base(id)
    {
        if (recallCaseId == Guid.Empty || objectId == Guid.Empty) throw new ArgumentException("Recall and source identifiers are required.");
        RecallCaseId = recallCaseId; ObjectType = objectType; ObjectId = objectId;
    }
    public Guid RecallCaseId { get; private set; }
    public TraceObjectType ObjectType { get; private set; }
    public Guid ObjectId { get; private set; }
}

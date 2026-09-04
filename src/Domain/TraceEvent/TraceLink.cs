using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;

namespace EdgIQ.MurimiOS.Domain.TraceEvent;

public sealed class TraceLink : TenantEntity
{
    private TraceLink() { }

    public TraceLink(Guid id, Guid eventId, TraceObjectType fromType, Guid fromId,
        TraceObjectType toType, Guid toId, TraceRelationshipType relationship) : base(id)
    {
        if (eventId == Guid.Empty || fromId == Guid.Empty || toId == Guid.Empty) throw new ArgumentException("Trace link identifiers are required.");
        if (fromType == toType && fromId == toId) throw new ArgumentException("A trace object cannot link to itself.");
        EventId = eventId;
        FromType = fromType;
        FromId = fromId;
        ToType = toType;
        ToId = toId;
        Relationship = relationship;
    }

    public Guid EventId { get; private set; }
    public TraceObjectType FromType { get; private set; }
    public Guid FromId { get; private set; }
    public TraceObjectType ToType { get; private set; }
    public Guid ToId { get; private set; }
    public TraceRelationshipType Relationship { get; private set; }
}

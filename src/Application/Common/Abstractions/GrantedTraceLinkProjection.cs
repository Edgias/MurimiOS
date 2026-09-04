using EdgIQ.MurimiOS.Domain.TraceEvent;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;

public sealed class GrantedTraceLinkProjection
{
    public Guid GrantId { get; set; }
    public Guid ProducerTenantId { get; set; }
    public Guid LinkId { get; set; }
    public Guid EventId { get; set; }
    public TraceObjectType FromType { get; set; }
    public Guid FromId { get; set; }
    public TraceObjectType ToType { get; set; }
    public Guid ToId { get; set; }
    public TraceRelationshipType Relationship { get; set; }
    public int Depth { get; set; }
}

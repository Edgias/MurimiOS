using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;

namespace EdgIQ.MurimiOS.Domain.TraceEvent;

public sealed class TraceEventObject : TenantEntity
{
    private TraceEventObject() { }

    public TraceEventObject(Guid id, Guid eventId, TraceObjectType objectType, Guid objectId,
        TraceObjectRole role, decimal? quantity = null, string? unit = null) : base(id)
    {
        if (eventId == Guid.Empty || objectId == Guid.Empty) throw new ArgumentException("Event and object identifiers are required.");
        if (quantity is <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity.HasValue != !string.IsNullOrWhiteSpace(unit)) throw new ArgumentException("Quantity and unit must be supplied together.");
        EventId = eventId;
        ObjectType = objectType;
        ObjectId = objectId;
        Role = role;
        if (quantity is { } value)
        {
            MeasuredQuantity measured = MeasurementUnits.Capture(value, unit!, unit);
            if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
                throw new ArgumentException("Trace quantities must use mass, volume, or count units.", nameof(unit));
            Quantity = measured.CanonicalValue;
            Unit = measured.CanonicalUnit;
            EnteredQuantity = measured.EnteredValue;
            EnteredUnit = measured.EnteredUnit;
        }
    }

    public Guid EventId { get; private set; }
    public TraceObjectType ObjectType { get; private set; }
    public Guid ObjectId { get; private set; }
    public TraceObjectRole Role { get; private set; }
    public decimal? Quantity { get; private set; }
    public string? Unit { get; private set; }
    public decimal? EnteredQuantity { get; private set; }
    public string? EnteredUnit { get; private set; }
}

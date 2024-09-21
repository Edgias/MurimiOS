namespace Edgias.MurimiOS.Domain.Entities;

public class Field(string name, Guid? locationId, Guid fieldMeasurementId, decimal usableArea,
    Guid? soilTypeId, Guid? ownershipTypeId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid? LocationId { get; private set; } = locationId;

    public Location? Location { get; private set; }

    public Guid FieldMeasurementId { get; private set; } = fieldMeasurementId;

    public FieldMeasurement FieldMeasurement { get; private set; } = default!;

    public decimal UsableArea { get; private set; } = usableArea;

    public Guid? SoilTypeId { get; private set; } = soilTypeId;

    public SoilType? SoilType { get; private set; }

    public Guid? OwnershipTypeId { get; private set; } = ownershipTypeId;

    public OwnershipType? OwnershipType { get; private set; }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateLocation(Guid locationId)
    {
        LocationId = locationId;
    }

    public void UpdateOwnershipType(Guid ownershipTypeId)
    {
        OwnershipTypeId = ownershipTypeId;
    }

    public void UpdateSoilType(Guid soilTypeId)
    {
        SoilTypeId = soilTypeId;
    }

    /// <summary>
    /// Updates the usable area of the field by subtracting areaAllocated from the currently available usable area.
    /// Throws an <see cref="ArgumentException" /> if <paramref name="areaAllocated" /> is zero.
    /// Throws an <see cref="InvalidOperationException" /> if <paramref name="areaAllocated" /> is greater that usable area.
    /// </summary>
    /// <param name="areaAllocated">An area of the field which has been allocated for use.</param>
    public void UpdateUsableArea(decimal areaAllocated)
    {
        Guard.AgainstZero(areaAllocated, nameof(areaAllocated));

        if (areaAllocated > UsableArea)
        {
            throw new InvalidOperationException($"Area allocated is greater that available usable area({UsableArea}).");
        }

        UsableArea -= areaAllocated;
    }
}

using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class Field : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public Guid? LocationId { get; private set; }

        public Location Location { get; private set; }

        public Guid FieldMeasurementId { get; private set; }

        public FieldMeasurement FieldMeasurement { get; private set; }

        public decimal UsableArea { get; private set; }

        public Guid? SoilTypeId { get; private set; }

        public SoilType SoilType { get; private set; }

        public Guid? OwnershipTypeId { get; private set; }

        public OwnershipType OwnershipType { get; private set; }

        public Field(string name, Guid? locationId, Guid fieldMeasurementId, decimal usableArea, Guid? soilTypeId, Guid? ownershipTypeId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(fieldMeasurementId, nameof(fieldMeasurementId));
            Guard.AgainstZero(usableArea, nameof(usableArea));

            Name = name;
            LocationId = locationId;
            FieldMeasurementId = fieldMeasurementId;
            UsableArea = usableArea;
            SoilTypeId = soilTypeId;
            OwnershipTypeId = ownershipTypeId;
        }

        public void UpdateName(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));

            Name = name;
        }

        public void UpdateLocation(Guid locationId)
        {
            Guard.AgainstNull(locationId, nameof(locationId));

            LocationId = locationId;
        }

        public void UpdateOwnershipType(Guid ownershipTypeId)
        {
            Guard.AgainstNull(ownershipTypeId, nameof(ownershipTypeId));

            OwnershipTypeId = ownershipTypeId;
        }

        public void UpdateSoilType(Guid soilTypeId)
        {
            Guard.AgainstNull(soilTypeId, nameof(soilTypeId));

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
}

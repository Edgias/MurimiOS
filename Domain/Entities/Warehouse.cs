using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class Warehouse : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public Guid LocationId { get; private set; }

        public Location Location { get; private set; }

        public Warehouse(string name, Guid locationId)
        {
            SetData(name, locationId);
        }

        public void UpdateDetails(string name, Guid locationId)
        {
            SetData(name, locationId);
        }

        private void SetData(string name, Guid locationId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(locationId, nameof(locationId));

            Name = name;
            LocationId = locationId;
        }
    }
}

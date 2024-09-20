using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class UnitMeasurement : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public Guid UnitGroupId { get; private set; }

        public UnitGroup UnitGroup { get; private set; }

        public UnitMeasurement(string name, Guid unitGroupId)
        {
            SetData(name, unitGroupId);
        }

        public void UpdateDetails(string name, Guid unitGroupId)
        {
            SetData(name, unitGroupId);
        }

        private void SetData(string name, Guid unitGroupId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(unitGroupId, nameof(unitGroupId));

            Name = name;
            UnitGroupId = unitGroupId;
        }
    }
}

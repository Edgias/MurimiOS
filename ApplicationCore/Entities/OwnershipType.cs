using Edgias.MurimiOS.Domain.SharedKernel;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class OwnershipType : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public OwnershipType(string name)
        {
            SetData(name);
        }

        public void UpdateDetails(string name)
        {
            SetData(name);
        }

        private void SetData(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));

            Name = name;
        }
    }
}

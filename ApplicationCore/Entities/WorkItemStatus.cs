using Edgias.MurimiOS.Domain.SharedKernel;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class WorkItemStatus : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public bool IsDefault { get; private set; }

        public WorkItemStatus(string name, bool isDefault)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));

            Name = name;
            IsDefault = isDefault;
        }

        public void UpdateDetails(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));

            Name = name;
        }

        public void MakeDefault()
        {
            IsDefault = true;
        }
    }
}

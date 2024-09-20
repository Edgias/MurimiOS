using Edgias.MurimiOS.Domain.SharedKernel;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class SeasonStatus : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public bool IsDefault { get; private set; }

        public SeasonStatus(string name, bool isDefault)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));

            Name = name;
            IsDefault = isDefault;
        }

        public void UpdateName(string name)
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

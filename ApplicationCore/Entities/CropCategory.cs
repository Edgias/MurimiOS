using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class CropCategory : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public CropCategory(string name, string description)
        {
            SetData(name, description);
        }

        public void UpdateDetails(string name, string description)
        {
            SetData(name, description);
        }

        private void SetData(string name, string description)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(description, nameof(description));

            Name = name;
            Description = description;
        }
    }
}

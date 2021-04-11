using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class WorkItemCategory : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public WorkItemCategory(string name)
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

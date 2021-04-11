using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class Tax : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public decimal Percentage { get; private set; }

        public Tax(string name, decimal percentage)
        {
            SetData(name, percentage);
        }

        public void UpdateDetails(string name, decimal percentage)
        {
            SetData(name, percentage);
        }

        private void SetData(string name, decimal percentage)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(percentage, nameof(percentage));

            Name = name;
            Percentage = percentage;
        }
    }
}

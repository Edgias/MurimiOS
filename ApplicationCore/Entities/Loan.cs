using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Loan : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public int Duration { get; private set; }

        public decimal PrincipalAmount { get; private set; }

        public decimal InterestRate { get; private set; }

        public decimal EffectiveRate { get; private set; }

        public Guid CurrencyId { get; private set; }

        public Currency Currency { get; private set; }

        public Loan(string name, string description, int duration, decimal principalAmount, decimal interestRate, 
            decimal effectiveRate, Guid currencyId)
        {
            SetData(name, description, duration, principalAmount, interestRate, effectiveRate);
            CurrencyId = currencyId;
        }

        public void UpdateDetails(string name, string description, int duration, decimal principalAmount, decimal interestRate,
            decimal effectiveRate)
        {
            SetData(name, description, duration, principalAmount, interestRate, effectiveRate);
        }

        private void SetData(string name, string description, int duration, decimal principalAmount, decimal interestRate,
            decimal effectiveRate)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(duration, nameof(duration));
            Guard.AgainstZero(principalAmount, nameof(principalAmount));
            Guard.AgainstZero(interestRate, nameof(interestRate));
            Guard.AgainstZero(effectiveRate, nameof(effectiveRate));

            Name = name;
            Description = description;
            Duration = duration;
            PrincipalAmount = principalAmount;
            InterestRate = interestRate;
            EffectiveRate = effectiveRate;
        }
    }
}

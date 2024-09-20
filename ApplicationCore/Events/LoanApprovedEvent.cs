using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Events
{
    public class LoanApprovedEvent : BaseDomainEvent
    {
        public string Name { get; private set; }

        public decimal Amount { get; private set; }

        public DateTimeOffset DateApproved { get; private set; }

        public LoanApprovedEvent(string name, decimal amount, DateTime dateApproved)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(amount, nameof(amount));
            Guard.AgainstNull(dateApproved, nameof(dateApproved));

            Name = name;
            Amount = amount;
            DateApproved = dateApproved;
        }
    }
}

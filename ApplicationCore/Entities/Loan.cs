using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Loan : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public decimal PrincipalAmount { get; set; }

        public decimal InterestRate { get; set; }

        public decimal EffectiveRate { get; set; }

        public Guid CurrencyId { get; set; }

        public Currency Currency { get; set; }
    }
}

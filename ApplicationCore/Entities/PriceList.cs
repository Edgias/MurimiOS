using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities
{
    public class PriceList : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public decimal UnitPrice { get; private set; }

        public Guid ItemId { get; private set; }

        public Item Item { get; private set; }

        public Guid CurrencyId { get; private set; }

        public Currency Currency { get; private set; }

        public PriceList(string name, decimal unitPrice, Guid itemId, Guid currencyId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(unitPrice, nameof(unitPrice));
            Guard.AgainstNull(currencyId, nameof(currencyId));
            Guard.AgainstNull(itemId, nameof(itemId));

            Name = name;
            UnitPrice = unitPrice;
            ItemId = itemId;
            CurrencyId = currencyId;
        }

        public void UpdateDetails(string name, decimal unitPrice)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(unitPrice, nameof(unitPrice));

            Name = name;
            UnitPrice = unitPrice;
        }

        public void UpdateCurrency(Guid currencyId)
        {
            Guard.AgainstNull(currencyId, nameof(currencyId));

            CurrencyId = currencyId;
        }

    }
}

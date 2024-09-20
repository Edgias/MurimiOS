using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities.QuotationAggregate
{
    public class QuotationItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public ItemQuoted ItemQuoted { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; private set; }

        private QuotationItem()
        {
            // Required by EF
        }

        public QuotationItem(ItemQuoted itemQuoted, decimal unitPrice, int units, Guid? taxId)
        {
            Guard.AgainstZero(units, nameof(units));
            Guard.AgainstZero(unitPrice, nameof(unitPrice));
            Guard.AgainstNull(itemQuoted, nameof(itemQuoted));

            ItemQuoted = itemQuoted;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

        public void ChangeQuantity(int units)
        {
            Guard.AgainstZero(units, nameof(units));

            Units = units;
        }

        public void ChangeUnitPrice(decimal unitPrice)
        {
            Guard.AgainstZero(unitPrice, nameof(unitPrice));

            UnitPrice = unitPrice;
        }
    }
}

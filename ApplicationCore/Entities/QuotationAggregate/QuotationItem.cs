using System;

namespace Murimi.ApplicationCore.Entities.QuotationAggregate
{
    public class QuotationItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public ItemQuoted ItemQuoted { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; set; }

        public Guid QuotationId { get; set; }

        public Quotation Quotation { get; set; }

        private QuotationItem()
        {
        }

        public QuotationItem(ItemQuoted itemQuoted, decimal unitPrice, int units, Guid? taxId)
        {
            ItemQuoted = itemQuoted;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

        public void ChangeQuantity(int units)
        {
            Units = units;
        }

        public void ChangeUnitPrice(decimal unitPrice)
        {
            UnitPrice = unitPrice;
        }
    }
}

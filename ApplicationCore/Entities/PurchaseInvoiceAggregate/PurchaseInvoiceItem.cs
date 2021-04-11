using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities.PurchaseInvoiceAggregate
{
    public class PurchaseInvoiceItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public InvoicedItem InvoicedItem { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; private set; }

        private PurchaseInvoiceItem()
        {
            // Required by EF
        }

        public PurchaseInvoiceItem(InvoicedItem invoicedItem, decimal unitPrice, int units, Guid? taxId)
        {
            Guard.AgainstZero(unitPrice, nameof(unitPrice));
            Guard.AgainstZero(units, nameof(units));
            Guard.AgainstNull(invoicedItem, nameof(invoicedItem));

            InvoicedItem = invoicedItem;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

    }
}

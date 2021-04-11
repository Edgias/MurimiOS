using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities.SalesInvoiceAggregate
{
    public class SalesInvoiceItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public InvoicedItem InvoicedItem { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; private set; }

        private SalesInvoiceItem()
        {
        }

        public SalesInvoiceItem(InvoicedItem invoicedItem, decimal unitPrice, int units, Guid? taxId)
        {
            Guard.AgainstZero(units, nameof(units));
            Guard.AgainstZero(unitPrice, nameof(unitPrice));
            Guard.AgainstNull(invoicedItem, nameof(invoicedItem));

            InvoicedItem = invoicedItem;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

    }
}

using System;

namespace Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate
{
    public class SalesInvoiceItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public InvoicedItem InvoicedItem { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; set; }

        public Guid SalesInvoiceId { get; set; }

        public SalesInvoice SalesInvoice { get; set; }

        private SalesInvoiceItem()
        {
        }

        public SalesInvoiceItem(InvoicedItem invoicedItem, decimal unitPrice, int units, Guid? taxId)
        {
            InvoicedItem = invoicedItem;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

    }
}

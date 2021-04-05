using System;

namespace Edgias.Agrik.ApplicationCore.Entities.PurchaseInvoiceAggregate
{
    public class PurchaseInvoiceItem : BaseEntity
    {
        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public InvoicedItem InvoicedItem { get; private set; }

        public Guid? TaxId { get; private set; }

        public Tax Tax { get; set; }

        public Guid PurchaseInvoiceId { get; set; }

        public PurchaseInvoice PurchaseInvoice { get; set; }

        private PurchaseInvoiceItem()
        {
        }

        public PurchaseInvoiceItem(InvoicedItem invoicedItem, decimal unitPrice, int units, Guid? taxId)
        {
            InvoicedItem = invoicedItem;
            UnitPrice = unitPrice;
            Units = units;
            TaxId = taxId;
        }

    }
}

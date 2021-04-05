using System;

namespace Edgias.Agrik.ApplicationCore.Entities.PurchaseInvoiceAggregate
{
    public class InvoicedItem
    {
        public Guid ItemId { get; private set; }

        public string ItemName { get; private set; }

        public string ItemDescription { get; private set; }

        private InvoicedItem()
        {

        }

        public InvoicedItem(Guid productId, string productName, string productDescription)
        {
            ItemId = productId;
            ItemName = productName;
            ItemDescription = productDescription;
        }
    }
}

using System;

namespace Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate
{
    public class InvoicedItem
    {
        public Guid ItemId { get; private set; }

        public string ItemName { get; private set; }

        public string ItemDescription { get; private set; }

        private InvoicedItem()
        {

        }

        public InvoicedItem(Guid itemId, string itemName, string itemDescription)
        {
            ItemId = itemId;
            ItemName = itemName;
            ItemDescription = itemDescription;
        }
    }
}

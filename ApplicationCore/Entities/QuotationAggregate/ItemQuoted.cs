using System;

namespace Edgias.Agrik.ApplicationCore.Entities.QuotationAggregate
{
    public class ItemQuoted
    {
        public Guid ItemId { get; private set; }

        public string ItemName { get; private set; }

        public string ItemDescription { get; private set; }

        private ItemQuoted()
        {

        }

        public ItemQuoted(Guid itemId, string itemName, string itemDescription)
        {
            ItemId = itemId;
            ItemName = itemName;
            ItemDescription = itemDescription;
        }
    }
}

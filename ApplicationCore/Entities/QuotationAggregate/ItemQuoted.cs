using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities.QuotationAggregate
{
    public class ItemQuoted // Value Object
    {
        public Guid ItemId { get; private set; }

        public string ItemName { get; private set; }

        public string ItemDescription { get; private set; }

        private ItemQuoted()
        {
            // Required by EF
        }

        public ItemQuoted(Guid itemId, string itemName, string itemDescription)
        {
            Guard.AgainstNull(itemId, nameof(itemId));
            Guard.AgainstNullOrEmpty(itemName, nameof(itemName));
            Guard.AgainstNullOrEmpty(itemDescription, nameof(itemDescription));

            ItemId = itemId;
            ItemName = itemName;
            ItemDescription = itemDescription;
        }
    }
}

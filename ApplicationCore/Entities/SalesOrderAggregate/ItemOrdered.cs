using System;

namespace Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate
{
    public class ItemOrdered
    {
        public Guid ItemId { get; private set; }

        public string ItemName { get; private set; }

        private ItemOrdered()
        {

        }

        public ItemOrdered(Guid productId, string productName)
        {
            ItemId = productId;
            ItemName = productName;
        }
    }
}

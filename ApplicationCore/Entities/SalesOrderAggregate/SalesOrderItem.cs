using System;

namespace Murimi.ApplicationCore.Entities.SalesOrderAggregate
{
    public class SalesOrderItem : BaseEntity
    {
        public ItemOrdered ItemOrdered { get; private set; }

        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public Guid SalesOrderId { get; set; }

        public SalesOrder SalesOrder { get; set; }

        private SalesOrderItem()
        {
            // required by EF
        }

        public SalesOrderItem(ItemOrdered itemOrdered, decimal unitPrice, int units)
        {
            ItemOrdered = itemOrdered;
            UnitPrice = unitPrice;
            Units = units;
        }
    }
}

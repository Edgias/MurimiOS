using Murimi.ApplicationCore.Entities.QuotationAggregate;
using System;
using System.Collections.Generic;

namespace Murimi.ApplicationCore.Entities.SalesOrderAggregate
{
    public class SalesOrder : BaseEntity
    {
        public string Name { get; private set; }

        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;

        public Address ShipToAddress { get; private set; }

        public Guid? CustomerId { get; private set; }

        public Customer Customer { get; private set; }

        public Guid? QuotationId { get; private set; }

        public Quotation Quotation { get; private set; }

        private readonly List<SalesOrderItem> _salesOrderItems = new List<SalesOrderItem>();

        public IReadOnlyCollection<SalesOrderItem> SalesOrderItems => _salesOrderItems.AsReadOnly();

        private SalesOrder()
        {
        }

        public SalesOrder(string name, Guid? customerId, Guid? quotationId, Address shipToAddress)
        {
            Name = name;
            CustomerId = customerId;
            QuotationId = quotationId;
            ShipToAddress = shipToAddress;
        }

        public SalesOrder(string name, Guid? customerId, Guid? quotationId, Address shipToAddress, List<SalesOrderItem> salesOrderItems)
        {
            Name = name;
            CustomerId = customerId;
            QuotationId = quotationId;
            ShipToAddress = shipToAddress;
            _salesOrderItems = salesOrderItems;
        }

        public void AddItem(SalesOrderItem salesOrderItem)
        {
            _salesOrderItems.Add(salesOrderItem);
        }

        public decimal Total()
        {
            decimal total = 0m;

            foreach (SalesOrderItem salesOrderItem in _salesOrderItems)
            {
                total += salesOrderItem.UnitPrice * salesOrderItem.Units;
            }

            return total;
        }
    }
}

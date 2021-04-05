using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class SalesOrderSpecification : BaseSpecification<SalesOrder>
    {
        public SalesOrderSpecification(Guid salesOrderId)
            : base(so => so.Id == salesOrderId)
        {
            AddInclude(o => o.SalesOrderItems);
            AddInclude($"{nameof(SalesOrder.SalesOrderItems)}.{nameof(SalesOrderItem.ItemOrdered)}");
        }

        public SalesOrderSpecification(Guid? customerId)
            : base(so => so.CustomerId == customerId)
        {
            ApplyOrderByDescending(so => so.OrderDate);
        }

    }
}

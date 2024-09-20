using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;
using System;

namespace Edgias.MurimiOS.Domain.Specifications
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

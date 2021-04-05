using Murimi.ApplicationCore.Entities.SalesOrderAggregate;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class SalesOrderItemSpecification : BaseSpecification<SalesOrderItem>
    {
        public SalesOrderItemSpecification(Guid salesOrderId) 
            : base(soi => soi.SalesOrderId == salesOrderId)
        {
        }
    }
}

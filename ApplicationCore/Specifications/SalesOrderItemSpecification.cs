using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class SalesOrderItemSpecification : BaseSpecification<SalesOrderItem>
    {
        public SalesOrderItemSpecification(Guid salesOrderId) 
            : base(soi => soi.SalesOrderId == salesOrderId)
        {
        }
    }
}

using Murimi.ApplicationCore.Entities.SalesInvoiceAggregate;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class SalesInvoiceItemSpecification : BaseSpecification<SalesInvoiceItem>
    {
        public SalesInvoiceItemSpecification(Guid salesInvoiceId) 
            : base(sii => sii.SalesInvoiceId == salesInvoiceId)
        {
        }
    }
}

using Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class SalesInvoiceItemSpecification : BaseSpecification<SalesInvoiceItem>
    {
        public SalesInvoiceItemSpecification(Guid salesInvoiceId) 
            : base(sii => sii.SalesInvoiceId == salesInvoiceId)
        {
        }
    }
}

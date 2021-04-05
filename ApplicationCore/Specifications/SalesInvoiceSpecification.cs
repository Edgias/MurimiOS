using Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class SalesInvoiceSpecification : BaseSpecification<SalesInvoice>
    {
        public SalesInvoiceSpecification(Guid salesInvoiceId)
            : base(si => si.Id == salesInvoiceId)
        {
            AddInclude(o => o.InvoiceItems);
            AddInclude($"{nameof(SalesInvoice.InvoiceItems)}.{nameof(SalesInvoiceItem.InvoicedItem)}");
            AddInclude($"{nameof(SalesInvoice.InvoiceItems)}.{nameof(SalesInvoiceItem.Tax)}");
        }

        public SalesInvoiceSpecification(Guid? customerId)
            : base(si => si.CustomerId == customerId)
        {
            ApplyOrderByDescending(i => i.InvoiceDate);
        }

    }
}

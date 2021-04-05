using Murimi.ApplicationCore.Entities.QuotationAggregate;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class QuotationSpecification : BaseSpecification<Quotation>
    {
        public QuotationSpecification()
            : base(criteria: null)
        {
            AddInclude(q => q.Customer);            
            ApplyOrderByDescending(q => q.QuotationDate);
        }

        public QuotationSpecification(Guid quotationId)
            : base(q => q.Id == quotationId)
        {
            AddInclude(q => q.Customer);
            AddInclude(o => o.QuotationItems);
            AddInclude($"{nameof(Quotation.QuotationItems)}.{nameof(QuotationItem.ItemQuoted)}");
            AddInclude($"{nameof(Quotation.QuotationItems)}.{nameof(QuotationItem.Tax)}");
            ApplyOrderByDescending(q => q.QuotationDate);
        }

        public QuotationSpecification(Guid? customerId)
            : base(q => q.CustomerId == customerId)
        {
            ApplyOrderByDescending(q => q.QuotationDate);
        }

    }
}

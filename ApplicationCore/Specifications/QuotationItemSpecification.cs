using Murimi.ApplicationCore.Entities.QuotationAggregate;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class QuotationItemSpecification : BaseSpecification<QuotationItem>
    {
        public QuotationItemSpecification(Guid quotationId)
            : base(qi => qi.QuotationId == quotationId)
        {

        }
    }
}

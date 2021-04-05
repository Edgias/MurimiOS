using Edgias.Agrik.ApplicationCore.Entities.QuotationAggregate;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class QuotationItemSpecification : BaseSpecification<QuotationItem>
    {
        public QuotationItemSpecification(Guid quotationId)
            : base(qi => qi.QuotationId == quotationId)
        {

        }
    }
}

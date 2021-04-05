using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class CustomerSpecification : BaseSpecification<Customer>
    {
        public CustomerSpecification()
            : base(criteria: null)
        {
            ApplyOrderBy(c => c.Name);
        }

        public CustomerSpecification(Guid customerId)
            : base(c => c.Id == customerId)
        {
        }

        public CustomerSpecification(string searchString)
            : base(c => string.IsNullOrEmpty(searchString) ||
            c.Name.Contains(searchString) ||
            c.Telephone.Contains(searchString) ||
            c.ContactPerson.Contains(searchString) ||
            c.ContactPersonPhone.Contains(searchString) ||
            c.ContactPersonEmail.Contains(searchString))
        {
        }

        public CustomerSpecification(int skip, int take, string searchString) 
            : base(c => string.IsNullOrEmpty(searchString) ||
            c.Name.Contains(searchString) ||
            c.Telephone.Contains(searchString) ||
            c.ContactPerson.Contains(searchString) ||
            c.ContactPersonPhone.Contains(searchString) ||
            c.ContactPersonEmail.Contains(searchString))
        {
            ApplyOrderBy(c => c.Name);
            ApplyPaging(skip, take);
        }
    }
}

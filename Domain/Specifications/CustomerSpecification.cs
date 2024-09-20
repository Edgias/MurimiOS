﻿using Edgias.MurimiOS.Domain.Entities;
using System;

namespace Edgias.MurimiOS.Domain.Specifications
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
            c.Phone.Contains(searchString) ||
            c.ContactPerson.Contains(searchString) ||
            c.ContactPersonPhone.Contains(searchString) ||
            c.ContactPersonEmail.Contains(searchString))
        {
        }

        public CustomerSpecification(int skip, int take, string searchString) 
            : base(c => string.IsNullOrEmpty(searchString) ||
            c.Name.Contains(searchString) ||
            c.Phone.Contains(searchString) ||
            c.ContactPerson.Contains(searchString) ||
            c.ContactPersonPhone.Contains(searchString) ||
            c.ContactPersonEmail.Contains(searchString))
        {
            ApplyOrderBy(c => c.Name);
            ApplyPaging(skip, take);
        }
    }
}

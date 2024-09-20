﻿using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Domain.Specifications
{
    public class WorkItemCategorySpecification : BaseSpecification<WorkItemCategory>
    {
        public WorkItemCategorySpecification(string searchQuery)
            : base(wic => string.IsNullOrEmpty(searchQuery) || wic.Name.Contains(searchQuery))
        {
        }

        public WorkItemCategorySpecification(int skip, int take, string searchQuery) 
            : base(wic => string.IsNullOrEmpty(searchQuery) || wic.Name.Contains(searchQuery))
        {
            ApplyOrderBy(wic => wic.Name);
            ApplyPaging(skip, take);
        }
    }
}

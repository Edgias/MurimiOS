using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class WorkItemSubCategorySpecification : BaseSpecification<WorkItemSubCategory>
    {
        public WorkItemSubCategorySpecification(string searchQuery)
            : base(wisc => string.IsNullOrEmpty(searchQuery) || wisc.Name.Contains(searchQuery))
        {
        }

        public WorkItemSubCategorySpecification(Guid workItemCategoryId)
            : base(wisc => wisc.WorkItemCategoryId == workItemCategoryId)
        {
            ApplyOrderBy(wisc => wisc.Name);
        }

        public WorkItemSubCategorySpecification(int skip, int take, string searchQuery) 
            : base(wisc => string.IsNullOrEmpty(searchQuery) || wisc.Name.Contains(searchQuery))
        {
            AddInclude(wisc => wisc.WorkItemCategory);
            ApplyOrderBy(wisc => wisc.Name);
            ApplyPaging(skip, take);
        }

        public WorkItemSubCategorySpecification(Guid workItemCategoryId, string searchQuery)
            : base(wisc => wisc.WorkItemCategoryId == workItemCategoryId && string.IsNullOrEmpty(searchQuery) || wisc.Name.Contains(searchQuery))
        {
        }

        public WorkItemSubCategorySpecification(Guid workItemCategoryId, int skip, int take, string searchQuery)
            : base(wisc => wisc.WorkItemCategoryId == workItemCategoryId && string.IsNullOrEmpty(searchQuery) || wisc.Name.Contains(searchQuery))
        {
            ApplyOrderBy(wisc => wisc.Name);
            ApplyPaging(skip, take);
        }
    }
}

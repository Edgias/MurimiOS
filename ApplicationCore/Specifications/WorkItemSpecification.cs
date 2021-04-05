using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class WorkItemSpecification : BaseSpecification<WorkItem>
    {
        public WorkItemSpecification(string searchQuery)
            : base(wi => string.IsNullOrEmpty(searchQuery) || wi.Name.Contains(searchQuery) || wi.Description.Contains(searchQuery))
        {
        }

        public WorkItemSpecification(Guid seasonId)
            : base(wi => wi.SeasonId == seasonId)
        {
            AddInclude(wi => wi.WorkItemCategory);
            AddInclude(wi => wi.WorkItemSubCategory);
            AddInclude(wi => wi.WorkItemStatus);
            AddInclude(wi => wi.CropProduction);
            ApplyOrderBy(wi => wi.Name);
        }

        public WorkItemSpecification(int skip, int take, string searchQuery) 
            : base(wi => string.IsNullOrEmpty(searchQuery) || wi.Name.Contains(searchQuery) || wi.Description.Contains(searchQuery))
        {
            AddInclude(wi => wi.WorkItemCategory);
            AddInclude(wi => wi.WorkItemSubCategory);
            AddInclude(wi => wi.WorkItemStatus);
            AddInclude(wi => wi.CropProduction);
            AddInclude(wi => wi.Season);
            ApplyOrderBy(wi => wi.Name);
            ApplyPaging(skip, take);
        }

        public WorkItemSpecification(Guid seasonId, string searchQuery)
            : base(wi => wi.SeasonId == seasonId && string.IsNullOrEmpty(searchQuery) || wi.Name.Contains(searchQuery) || wi.Description.Contains(searchQuery))
        {
        }

        public WorkItemSpecification(Guid seasonId, int skip, int take, string searchQuery)
            : base(wi => wi.SeasonId == seasonId && string.IsNullOrEmpty(searchQuery) || wi.Name.Contains(searchQuery) || wi.Description.Contains(searchQuery))
        {
            AddInclude(wi => wi.WorkItemCategory);
            AddInclude(wi => wi.WorkItemSubCategory);
            AddInclude(wi => wi.WorkItemStatus);
            AddInclude(wi => wi.CropProduction);
            ApplyOrderBy(wi => wi.Name);
            ApplyPaging(skip, take);
        }
    }
}

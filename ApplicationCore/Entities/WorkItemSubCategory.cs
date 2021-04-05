using System;

namespace Murimi.ApplicationCore.Entities
{
    public class WorkItemSubCategory : BaseEntity
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public WorkItemCategory WorkItemCategory { get; set; }
    }
}

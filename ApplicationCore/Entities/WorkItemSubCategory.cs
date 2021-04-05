using System;

namespace Edgias.Agrik.ApplicationCore.Entities
{
    public class WorkItemSubCategory : BaseEntity
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public WorkItemCategory WorkItemCategory { get; set; }
    }
}

using System;

namespace Murimi.ApplicationCore.Entities
{
    public class WorkItem : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Guid WorkItemStatusId { get; set; }

        public WorkItemStatus WorkItemStatus { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public WorkItemCategory WorkItemCategory { get; set; }

        public Guid? WorkItemSubCategoryId { get; set; }

        public WorkItemSubCategory WorkItemSubCategory { get; set; }

        public Guid? FieldId { get; set; }

        public Field Field { get; set; }

        public Guid SeasonId { get; set; }

        public Season Season { get; set; }

        public Guid? CropProductionId { get; set; }

        public CropProduction CropProduction { get; set; }
    }
}

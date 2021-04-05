using System;

namespace Edgias.Agrik.API.Models.View
{
    public class WorkItemApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Guid WorkItemStatusId { get; set; }

        public string WorkItemStatus { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public string WorkItemCategory { get; set; }

        public Guid? WorkItemSubCategoryId { get; set; }

        public string WorkItemSubCategory { get; set; }

        public Guid? FieldId { get; set; }

        public string Field { get; set; }

        public Guid SeasonId { get; set; }

        public string Season { get; set; }

        public Guid? CropProductionId { get; set; }

        public string CropProduction { get; set; }
    }
}

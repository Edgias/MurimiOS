using System;

namespace Murimi.API.Models.Requests
{
    public class WorkItemRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Guid WorkItemStatusId { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public Guid? WorkItemSubCategoryId { get; set; }

        public Guid? FieldId { get; set; }

        public Guid SeasonId { get; set; }

        public Guid? CropProductionId { get; set; }
    }
}

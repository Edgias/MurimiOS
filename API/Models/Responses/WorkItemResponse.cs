using System;

namespace Murimi.API.Models.Responses
{
    public class WorkItemResponse : BaseResponse
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Guid WorkItemStatusId { get; set; }

        public string WorkItemStatusName { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public string WorkItemCategoryName { get; set; }

        public Guid? WorkItemSubCategoryId { get; set; }

        public string WorkItemSubCategoryName { get; set; }

        public Guid? FieldId { get; set; }

        public string FieldName { get; set; }

        public Guid SeasonId { get; set; }

        public string SeasonName { get; set; }

        public Guid? CropProductionId { get; set; }

        public string CropProductionName { get; set; }
    }
}

using System;

namespace Murimi.API.Models.Responses
{
    public class WorkItemSubCategoryResponse : BaseResponse
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public string WorkItemCategoryName { get; set; }
    }
}

using System;

namespace Murimi.API.Models.Requests
{
    public class WorkItemSubCategoryRequest
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }
    }
}

using System;

namespace Murimi.API.Models.Request
{
    public class WorkItemSubCategoryRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }
    }
}

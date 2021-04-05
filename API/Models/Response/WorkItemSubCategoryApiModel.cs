using System;

namespace Murimi.API.Models.Response
{
    public class WorkItemSubCategoryApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public string WorkItemCategory { get; set; }
    }
}

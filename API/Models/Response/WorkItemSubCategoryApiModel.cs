using System;

namespace Edgias.Agrik.API.Models.View
{
    public class WorkItemSubCategoryApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }

        public string WorkItemCategory { get; set; }
    }
}

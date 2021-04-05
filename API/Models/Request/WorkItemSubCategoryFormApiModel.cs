using System;

namespace Edgias.Agrik.API.Models.Form
{
    public class WorkItemSubCategoryRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid WorkItemCategoryId { get; set; }
    }
}

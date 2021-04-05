using System;

namespace Edgias.Agrik.API.Models.Form
{
    public class CropRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public Guid CropUnitId { get; set; }
    }
}

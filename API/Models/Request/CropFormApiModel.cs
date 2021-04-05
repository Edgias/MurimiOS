using System;

namespace Murimi.API.Models.Request
{
    public class CropRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public Guid CropUnitId { get; set; }
    }
}

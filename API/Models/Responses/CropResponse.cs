using System;

namespace Murimi.API.Models.Responses
{
    public class CropResponse : BaseResponse
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public string CropCategoryName { get; set; }

        public Guid CropUnitId { get; set; }

        public string CropUnitName { get; set; }
    }
}

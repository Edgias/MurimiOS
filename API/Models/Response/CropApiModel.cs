using System;

namespace Edgias.Agrik.API.Models.View
{
    public class CropApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public string CropCategory { get; set; }

        public Guid CropUnitId { get; set; }

        public string CropUnit { get; set; }
    }
}

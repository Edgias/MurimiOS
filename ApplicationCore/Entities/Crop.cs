using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Crop : BaseEntity
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public CropCategory CropCategory { get; set; }

        public Guid CropUnitId { get; set; }

        public CropUnit CropUnit { get; set; }
    }
}

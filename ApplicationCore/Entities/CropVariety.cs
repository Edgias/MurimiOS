using System;

namespace Murimi.ApplicationCore.Entities
{
    public class CropVariety : BaseEntity
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }

        public Crop Crop { get; set; }
    }
}

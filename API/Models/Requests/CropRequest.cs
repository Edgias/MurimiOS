using System;

namespace Murimi.API.Models.Requests
{
    public class CropRequest
    {
        public string Name { get; set; }

        public Guid CropCategoryId { get; set; }

        public Guid CropUnitId { get; set; }
    }
}

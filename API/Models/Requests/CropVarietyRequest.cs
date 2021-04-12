using System;

namespace Murimi.API.Models.Requests
{
    public class CropVarietyRequest
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }
    }
}

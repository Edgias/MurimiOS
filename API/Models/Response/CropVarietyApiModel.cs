using System;

namespace Murimi.API.Models.Response
{
    public class CropVarietyApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }

        public string Crop { get; set; }
    }
}

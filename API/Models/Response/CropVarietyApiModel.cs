using System;

namespace Edgias.Agrik.API.Models.View
{
    public class CropVarietyApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }

        public string Crop { get; set; }
    }
}

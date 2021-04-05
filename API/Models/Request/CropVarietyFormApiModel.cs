using System;

namespace Murimi.API.Models.Request
{
    public class CropVarietyRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }
    }
}

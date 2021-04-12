using System;

namespace Murimi.API.Models.Responses
{
    public class CropVarietyResponse : BaseResponse
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }

        public string CropName { get; set; }
    }
}

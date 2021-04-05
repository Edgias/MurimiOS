using System;

namespace Edgias.Agrik.API.Models.Form
{
    public class CropVarietyRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }
    }
}

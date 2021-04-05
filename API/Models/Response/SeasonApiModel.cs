using System;

namespace Edgias.Agrik.API.Models.View
{
    public class SeasonApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid StatusId { get; set; }

        public string Status { get; set; }
    }
}

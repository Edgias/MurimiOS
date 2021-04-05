using System;

namespace Murimi.API.Models.Request
{
    public class SeasonRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid StatusId { get; set; }
    }
}

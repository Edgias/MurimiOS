using System;

namespace Murimi.API.Models.Requests
{
    public class SeasonRequest
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid StatusId { get; set; }
    }
}

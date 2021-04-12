using System;

namespace Murimi.API.Models.Responses
{
    public class SeasonResponse : BaseResponse
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid StatusId { get; set; }

        public string Status { get; set; }
    }
}

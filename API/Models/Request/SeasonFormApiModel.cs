using System;

namespace Edgias.Agrik.API.Models.Form
{
    public class SeasonRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid StatusId { get; set; }
    }
}

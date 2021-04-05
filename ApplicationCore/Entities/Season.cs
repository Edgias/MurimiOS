using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Season : BaseEntity
    {
        public string Name { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid SeasonStatusId { get; set; }

        public SeasonStatus SeasonStatus { get; set; }
    }
}

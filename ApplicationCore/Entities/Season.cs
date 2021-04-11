using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Season : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset EndDate { get; private set; }

        public Guid SeasonStatusId { get; private set; }

        public SeasonStatus SeasonStatus { get; private set; }

        public Season(string name, DateTimeOffset startDate, DateTimeOffset endDate, Guid seasonStatusId)
        {
            SetData(name, startDate, endDate);
            SeasonStatusId = seasonStatusId;
        }

        public void UpdateDetails(string name, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            SetData(name, startDate, endDate);
        }

        public void UpdateSeasonStatus(Guid seasonStatusId)
        {
            Guard.AgainstNull(seasonStatusId, nameof(seasonStatusId));

            SeasonStatusId = seasonStatusId;
        }

        private void SetData(string name, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(startDate, nameof(startDate));
            Guard.AgainstNull(endDate, nameof(endDate));

            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

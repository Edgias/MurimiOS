namespace Edgias.MurimiOS.Domain.Entities;

public class Season(string name, DateTimeOffset startDate, DateTimeOffset endDate) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public DateTimeOffset StartDate { get; private set; } = startDate;

    public DateTimeOffset EndDate { get; private set; } = endDate;

    public Guid SeasonStatusId { get; private set; } 

    public SeasonStatus SeasonStatus { get; private set; } = default!;

    public void Update(string name, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateSeasonStatus(Guid seasonStatusId)
    {
        SeasonStatusId = seasonStatusId;
    }

}



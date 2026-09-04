namespace EdgIQ.MurimiOS.Domain.YieldMeasurement;

public class YieldMeasurement(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void UpdateDetails(string name)
    {
        Name = name;
    }
}



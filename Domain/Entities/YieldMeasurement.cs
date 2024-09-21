namespace Edgias.MurimiOS.Domain.Entities;

public class YieldMeasurement(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void UpdateDetails(string name)
    {
        Name = name;
    }
}



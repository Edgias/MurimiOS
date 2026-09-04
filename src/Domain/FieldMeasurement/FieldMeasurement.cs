namespace EdgIQ.MurimiOS.Domain.FieldMeasurement;

public class FieldMeasurement(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }
}



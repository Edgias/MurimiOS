namespace EdgIQ.MurimiOS.Domain.CropUnit;

public class CropUnit(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}



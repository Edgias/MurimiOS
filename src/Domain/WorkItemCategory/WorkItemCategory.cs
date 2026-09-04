namespace EdgIQ.MurimiOS.Domain.WorkItemCategory;

public class WorkItemCategory(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}



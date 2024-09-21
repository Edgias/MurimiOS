namespace Edgias.MurimiOS.Domain.Entities;

public class WorkItemSubCategory(string name, Guid workItemCategoryId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid WorkItemCategoryId { get; private set; } = workItemCategoryId;

    public WorkItemCategory WorkItemCategory { get; private set; } = default!;

    public void Update(string name)
    {
        Name = name;
    }

    public void UpdateWorkItemCategory(Guid workItemCategoryId)
    {
        WorkItemCategoryId = workItemCategoryId;
    }
}



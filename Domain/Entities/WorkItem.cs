using Edgias.MurimiOS.Domain.Entities.CropProductionAggregate;

namespace Edgias.MurimiOS.Domain.Entities;

public class WorkItem(string name, string description, DateTimeOffset? startDate, DateTimeOffset? endDate,
        Guid workItemCategoryId, Guid? workItemSubCategoryId, Guid? fieldId, Guid seasonId, Guid? cropProductionId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Description { get; private set; } = description;

    public DateTimeOffset? StartDate { get; private set; } = startDate;

    public DateTimeOffset? EndDate { get; private set; } = endDate;

    public Guid WorkItemStatusId { get; private set; }

    public WorkItemStatus WorkItemStatus { get; private set; } = default!;

    public Guid WorkItemCategoryId { get; private set; } = workItemCategoryId;

    public WorkItemCategory WorkItemCategory { get; private set; } = default!;

    public Guid? WorkItemSubCategoryId { get; private set; } = workItemSubCategoryId;

    public WorkItemSubCategory? WorkItemSubCategory { get; private set; }

    public Guid? FieldId { get; private set; } = fieldId;

    public Field? Field { get; private set; }

    public Guid SeasonId { get; private set; } = seasonId;

    public Season Season { get; private set; } = default!;

    public Guid? CropProductionId { get; private set; } = cropProductionId;

    public CropProduction? CropProduction { get; private set; }

    public void Update(string name, string description, DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateCropProduction(Guid cropProductionId)
    {
        CropProductionId = cropProductionId;
    }

    public void UpdateField(Guid fieldId)
    {
        FieldId = fieldId;
    }

    public void UpdateWorkItemCategory(Guid workItemCategoryId)
    {
        WorkItemCategoryId = workItemCategoryId;
    }

    public void UpdateWorkItemSubCategory(Guid workItemSubCategoryId)
    {
        WorkItemSubCategoryId = workItemSubCategoryId;
    }

    public void UpdateWorkItemStatus(Guid workItemStatusId)
    {
        WorkItemStatusId = workItemStatusId;
    }
}



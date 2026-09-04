namespace Edgias.MurimiOS.API.WorkItemCategories;

public static class WorkItemCategoryMapper
{
    public static WorkItemCategoryResponse AsApiResponse(this WorkItemCategory entity)
    {
        WorkItemCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static WorkItemCategory ToEntity(this WorkItemCategoryRequest request)
    {
        WorkItemCategory entity = new(request.Name);

        return entity;
    }

    public static void Update(this WorkItemCategory entity, WorkItemCategoryRequest request)
    {
        entity.Update(request.Name);
    }
}


namespace Edgias.MurimiOS.API.WorkItemSubCategories;

public static class WorkItemSubCategoryMapper
{
    public static WorkItemSubCategoryResponse AsApiResponse(this WorkItemSubCategory entity)
    {
        WorkItemSubCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            WorkItemCategoryId = entity.WorkItemCategoryId,
            WorkItemCategoryName = entity.WorkItemCategory?.Name!
        };

        return response;
    }

    public static WorkItemSubCategory ToEntity(this WorkItemSubCategoryRequest request)
    {
        WorkItemSubCategory entity = new(request.Name, request.WorkItemCategoryId);

        return entity;
    }

    public static void Update(this WorkItemSubCategory entity, WorkItemSubCategoryRequest request)
    {
        entity.Update(request.Name);
    }
}


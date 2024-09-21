namespace Edgias.MurimiOS.API.WorkItems;

public static class WorkItemMapper
{
    public static WorkItemResponse AsApiResponse(this WorkItem entity)
    {
        WorkItemResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropProductionId = entity.CropProductionId,
            CropProductionName = entity.CropProduction?.Name!,
            WorkItemCategoryId = entity.WorkItemCategoryId,
            WorkItemCategoryName = entity.WorkItemCategory?.Name!,
            Description = entity.Description,
            EndDate = entity.EndDate,
            SeasonId = entity.SeasonId,
            SeasonName = entity.Season?.Name!,
            StartDate = entity.StartDate,
            WorkItemStatusId = entity.WorkItemStatusId,
            WorkItemStatusName = entity.WorkItemStatus?.Name!,
            WorkItemSubCategoryId = entity.WorkItemSubCategoryId,
            WorkItemSubCategoryName = entity.WorkItemSubCategory?.Name!,
            FieldId = entity.FieldId,
            FieldName = entity.Field?.Name!
        };

        return response;
    }

    public static WorkItem ToEntity(this WorkItemRequest request)
    {
        WorkItem entity = new(request.Name, request.Description, request.StartDate, request.EndDate, request.WorkItemCategoryId,
            request.WorkItemSubCategoryId, request.FieldId, request.SeasonId, request.CropProductionId);

        return entity;
    }

    public static void Update(this WorkItem entity, WorkItemRequest request)
    {
        entity.Update(request.Name, request.Description, request.StartDate, request.EndDate);
    }
}


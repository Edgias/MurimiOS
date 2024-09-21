namespace Edgias.MurimiOS.API.WorkItemStatuses;

public static class WorkItemStatusMapper
{
    public static WorkItemStatusResponse AsApiResponse(this WorkItemStatus entity)
    {
        WorkItemStatusResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDefault = entity.IsDefault
        };

        return response;
    }

    public static WorkItemStatus ToEntity(this WorkItemStatusRequest request)
    {
        WorkItemStatus entity = new(request.Name, request.IsDefault);

        return entity;
    }

    public static void Update(this WorkItemStatus entity, WorkItemStatusRequest request)
    {
        entity.Update(request.Name);
    }
}


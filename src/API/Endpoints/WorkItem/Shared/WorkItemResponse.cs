using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared;

public record WorkItemResponse : WorkItemModel
{
    [Required]
    public Guid Id { get; set; }

    public string WorkItemStatusName { get; set; } = default!;

    public string WorkItemCategoryName { get; set; } = default!;

    public string WorkItemSubCategoryName { get; set; } = default!;

    public string FieldName { get; set; } = default!;

    public string SeasonName { get; set; } = default!;

    public string CropProductionName { get; set; } = default!;


}


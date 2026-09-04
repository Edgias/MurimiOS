using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItems;

public record WorkItemModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public Guid WorkItemStatusId { get; set; }

    [Required]
    public Guid WorkItemCategoryId { get; set; }

    public Guid? WorkItemSubCategoryId { get; set; }

    public Guid? FieldId { get; set; }

    [Required]
    public Guid SeasonId { get; set; }

    public Guid? CropProductionId { get; set; }

}


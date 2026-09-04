using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;

public record WorkItemStatusModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public bool IsDefault { get; set; }

}


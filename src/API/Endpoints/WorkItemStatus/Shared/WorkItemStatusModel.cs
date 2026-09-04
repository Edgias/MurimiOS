using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemStatuses;

public record WorkItemStatusModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public bool IsDefault { get; set; }

}


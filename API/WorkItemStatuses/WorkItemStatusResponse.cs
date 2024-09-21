using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.WorkItemStatuses;

public record WorkItemStatusResponse : WorkItemStatusModel
{
    [Required]
    public Guid Id { get; set; }

}


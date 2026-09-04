using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;

public record WorkItemStatusResponse : WorkItemStatusModel
{
    [Required]
    public Guid Id { get; set; }

}


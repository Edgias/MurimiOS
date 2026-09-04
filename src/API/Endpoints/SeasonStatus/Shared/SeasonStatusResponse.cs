using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;

public record SeasonStatusResponse : SeasonStatusModel
{
    [Required]
    public Guid Id { get; set; }

}


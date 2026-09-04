using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Season.Shared;

public record SeasonResponse : SeasonModel
{
    [Required]
    public Guid Id { get; set; }

    public string SeasonStatusName { get; set; } = default!;

}


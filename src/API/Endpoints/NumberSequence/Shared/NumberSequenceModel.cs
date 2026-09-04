using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.NumberSequences;

public record NumberSequenceModel
{
    [Required]
    public string Entity { get; set; } = default!;

    public string Prefix { get; set; } = default!;

    public string Seperator { get; set; } = default!;

    [Required]
    public int StartingNumber { get; set; }

    public string Suffix { get; set; } = default!;


}


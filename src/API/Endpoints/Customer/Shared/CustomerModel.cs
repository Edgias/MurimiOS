using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Customers;

public record CustomerModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    [Required]
    public string Phone { get; set; } = default!;

    public string Website { get; set; } = default!;

    public string ContactPerson { get; set; } = default!;

    public string ContactPersonEmail { get; set; } = default!;

    public string ContactPersonPhone { get; set; } = default!;

    public Address BillingAddress { get; set; } = default!;

}


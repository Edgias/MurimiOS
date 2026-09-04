using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Customers;

public record CustomerResponse : CustomerModel
{
    [Required]
    public Guid Id { get; set; }

}


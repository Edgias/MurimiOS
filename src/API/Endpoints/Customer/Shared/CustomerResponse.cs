using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Customer.Shared;

public record CustomerResponse : CustomerModel
{
    [Required]
    public Guid Id { get; set; }

}


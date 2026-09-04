namespace Edgias.MurimiOS.API.Customers;

public static class CustomerMapper
{
    public static CustomerResponse AsApiResponse(this Customer entity)
    {
        CustomerResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            ContactPerson = entity.ContactPerson,
            ContactPersonEmail = entity.ContactPersonEmail,
            ContactPersonPhone = entity.ContactPersonPhone,
            Website = entity.Website,
            BillingAddress = entity.BillingAddress
        };

        return response;
    }

    public static Customer ToEntity(this CustomerRequest request)
    {
        Customer entity = new(request.Name, request.Phone, request.Email,
            request.Website, request.ContactPerson, request.ContactPersonEmail,
            request.ContactPersonPhone, request.BillingAddress);

        return entity;
    }

    public static void Update(this Customer entity, CustomerRequest request)
    {
        entity.Update(request.Name, request.Phone, request.Email, request.Website,
            request.ContactPerson, request.ContactPersonEmail, request.ContactPersonPhone,
            request.BillingAddress);
    }
}


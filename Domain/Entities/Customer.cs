namespace Edgias.MurimiOS.Domain.Entities;

public class Customer(string name, string phone, string email, string website, string contactPerson,
        string contactPersonEmail, string contactPersonPhone, Address billingAddress) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Email { get; private set; } = email;

    public string Phone { get; private set; } = phone;

    public string Website { get; private set; } = website;

    public string ContactPerson { get; private set; } = contactPerson;

    public string ContactPersonEmail { get; private set; } = contactPersonEmail;

    public string ContactPersonPhone { get; private set; } = contactPersonPhone;

    public Address BillingAddress { get; private set; } = billingAddress;

    public void Update(string name, string phone, string email, string website, string contactPerson,
        string contactPersonEmail, string contactPersonPhone, Address billingAddress)
    {
        Name = name;
        Phone = phone;
        Email = email;
        Website = website;
        ContactPerson = contactPerson;
        ContactPersonEmail = contactPersonEmail;
        ContactPersonPhone = contactPersonPhone;
        BillingAddress = billingAddress;
    }

}



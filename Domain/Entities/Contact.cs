namespace Edgias.MurimiOS.Domain.Entities;

public class Contact(ContactType contactType, string firstName, string lastName, 
    string email, string primaryPhone, string mobileNumber, string company, 
    string tags, Address address) : BaseEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = firstName;

    public string LastName { get; private set; } = lastName;

    public string Email { get; private set; } = email;

    public ContactType ContactType { get; private set; } = contactType;

    public string PrimaryPhone { get; private set; } = primaryPhone;

    public string MobileNumber { get; private set; } = mobileNumber;

    public string Company { get; private set; } = company;

    public string Tags { get; private set; } = tags;

    public Address Address { get; private set; } = address;

    public void Update(string firstName, string lastName, string email,
    string primaryPhone, string mobileNumber, string company, string tags, Address address)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PrimaryPhone = primaryPhone;
        MobileNumber = mobileNumber;
        Company = company;
        Tags = tags;
        Address = address;
    }
}



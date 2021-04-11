using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class Supplier : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Telephone { get; private set; }

        public string Email { get; private set; }

        public string Website { get; private set; }

        public string ContactPerson { get; private set; }

        public string ContactPersonEmail { get; private set; }

        public string ContactPersonPhone { get; private set; }

        public Supplier(string name, string telephone, string email, string website, string contactPerson, 
            string contactPersonEmail, string contactPersonPhone)
        {
            SetData(name, telephone, email, website, contactPerson, contactPersonEmail, contactPersonPhone);
        }

        public void UpdateDetails(string name, string telephone, string email, string website, string contactPerson,
            string contactPersonEmail, string contactPersonPhone)
        {
            SetData(name, telephone, email, website, contactPerson, contactPersonEmail, contactPersonPhone);
        }

        private void SetData(string name, string telephone, string email, string website, string contactPerson,
            string contactPersonEmail, string contactPersonPhone)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(email, nameof(email));
            Guard.AgainstNullOrEmpty(contactPerson, nameof(contactPerson));
            Guard.AgainstNullOrEmpty(contactPersonEmail, nameof(contactPersonEmail));

            Name = name;
            Telephone = telephone;
            Email = email;
            Website = website;
            ContactPerson = contactPerson;
            ContactPersonEmail = contactPersonEmail;
            ContactPersonPhone = contactPersonPhone;
        }
    }
}

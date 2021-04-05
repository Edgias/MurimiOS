namespace Murimi.ApplicationCore.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }

        public string Telephone { get; set; }

        public string Website { get; set; }

        public string ContactPerson { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhone { get; set; }

        public Address BillingAddress { get; set; }
    }
}

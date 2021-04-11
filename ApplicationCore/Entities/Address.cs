namespace Murimi.ApplicationCore
{
    public class Address // Value object
    {
        public string Street1 { get; private set; }

        public string Street2 { get; private set; }

        public string City { get; private set; }

        public string State { get; private set; }

        public string ZipCode { get; private set; }

        public string Country { get; private set; }

        public Address(string street1, string street2, string city, string state, string zipCode, string country)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }
    }
}

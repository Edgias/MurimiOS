namespace Murimi.API.Models.Responses
{
    public class LocationResponse : BaseResponse
    {
        public string Name { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }
    }
}

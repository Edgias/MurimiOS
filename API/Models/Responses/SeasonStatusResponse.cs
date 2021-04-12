namespace Murimi.API.Models.Responses
{
    public class SeasonStatusResponse : BaseResponse
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

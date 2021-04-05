namespace Murimi.API.Models.Request
{
    public class SeasonStatusRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

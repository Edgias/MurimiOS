namespace Murimi.API.Models.Response
{
    public class SeasonStatusApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

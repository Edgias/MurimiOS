namespace Edgias.Agrik.API.Models.Form
{
    public class SeasonStatusRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

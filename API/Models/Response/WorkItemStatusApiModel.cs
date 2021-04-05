namespace Murimi.API.Models.Response
{
    public class WorkItemStatusApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

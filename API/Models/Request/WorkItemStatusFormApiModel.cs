namespace Murimi.API.Models.Request
{
    public class WorkItemStatusRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

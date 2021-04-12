namespace Murimi.API.Models.Responses
{
    public class WorkItemStatusResponse : BaseResponse
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

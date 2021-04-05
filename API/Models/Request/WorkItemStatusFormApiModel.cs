namespace Edgias.Agrik.API.Models.Form
{
    public class WorkItemStatusRequestApiModel : BaseRequestApiModel
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}

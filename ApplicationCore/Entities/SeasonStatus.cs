namespace Murimi.ApplicationCore.Entities
{
    public class SeasonStatus : BaseEntity
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public void MakeDefault()
        {
            IsDefault = true;
        }
    }
}

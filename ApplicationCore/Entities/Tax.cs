namespace Murimi.ApplicationCore.Entities
{
    public class Tax : BaseEntity
    {
        public string Name { get; set; }

        public decimal Percentage { get; set; }

    }
}

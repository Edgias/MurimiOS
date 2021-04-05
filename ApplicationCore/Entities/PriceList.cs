using System;

namespace Murimi.ApplicationCore.Entities
{
    public class PriceList : BaseEntity
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        public Guid ItemId { get; set; }

        public Item Item { get; set; }

    }
}

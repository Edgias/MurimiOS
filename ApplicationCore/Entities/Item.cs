using System;

namespace Edgias.Agrik.ApplicationCore.Entities
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid ItemCategoryId { get; set; }

        public ItemCategory ItemCategory { get; set; }
    }
}

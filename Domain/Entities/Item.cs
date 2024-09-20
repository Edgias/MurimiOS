using Edgias.MurimiOS.Domain.SharedKernel;
using System;

namespace Edgias.MurimiOS.Domain.Entities
{
    public class Item : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid ItemCategoryId { get; set; }

        public ItemCategory ItemCategory { get; set; }
    }
}

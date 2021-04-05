using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }

        public Guid LocationId { get; set; }

        public Location Location { get; set; }
    }
}

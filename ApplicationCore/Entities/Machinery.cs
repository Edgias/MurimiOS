using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Machinery : BaseEntity
    {
        public string Name { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string RegistrationNumber { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public Guid MachineryCategoryId { get; set; }

        public MachineryCategory MachineryCategory { get; set; }
    }
}

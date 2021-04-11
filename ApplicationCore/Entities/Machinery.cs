using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Machinery : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Manufacturer { get; private set; }

        public string Model { get; private set; }

        public string RegistrationNumber { get; private set; }

        public int Year { get; private set; }

        public string Description { get; private set; }

        public Guid MachineryCategoryId { get; private set; }

        public MachineryCategory MachineryCategory { get; private set; }

        public Machinery(string name, string manufacturer, string model, string registrationNumber, 
            int year, string description, Guid machineCategoryId)
        {
            SetData(name, manufacturer, model, registrationNumber, year, description);
            MachineryCategoryId = machineCategoryId;
        }

        public void UpdateDetails(string name, string manufacturer, string model, string registrationNumber,
            int year, string description)
        {
            SetData(name, manufacturer, model, registrationNumber, year, description);
        }

        public void UpdateMachineCategory(Guid machineCategoryId)
        {
            Guard.AgainstNull(machineCategoryId, nameof(machineCategoryId));

            MachineryCategoryId = machineCategoryId;
        }

        private void SetData(string name, string manufacturer, string model, string registrationNumber,
            int year, string description)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(manufacturer, nameof(manufacturer));
            Guard.AgainstNullOrEmpty(model, nameof(model));
            Guard.AgainstNullOrEmpty(registrationNumber, nameof(registrationNumber));
            Guard.AgainstZero(year, nameof(year));


            Name = name;
            Manufacturer = manufacturer;
            Model = model;
            RegistrationNumber = registrationNumber;
            Year = year;
            Description = description;
        }

    }
}

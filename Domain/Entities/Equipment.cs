namespace Edgias.MurimiOS.Domain.Entities;

public class Equipment(string name, string manufacturer, string model, string registrationNumber,
        int year, string description, Guid equipmentCategoryId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Manufacturer { get; private set; } = manufacturer;

    public string Model { get; private set; } = model;

    public string RegistrationNumber { get; private set; } = registrationNumber;

    public int Year { get; private set; } = year;

    public string Description { get; private set; } = description;

    public Guid EquipmentCategoryId { get; private set; } = equipmentCategoryId;

    public EquipmentCategory EquipmentCategory { get; private set; } = default!;

    public void Update(string name, string manufacturer, string model,
        string registrationNumber, int year, string description)
    {
        Name = name;
        Manufacturer = manufacturer;
        Model = model;
        RegistrationNumber = registrationNumber;
        Year = year;
        Description = description;
    }

    public void UpdateEquipmentCategory(Guid machineCategoryId)
    {
        EquipmentCategoryId = machineCategoryId;
    }

}



namespace Edgias.MurimiOS.Domain.Entities;

public class Tax(string name, decimal percentage) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public decimal Percentage { get; private set; } = percentage;

    public void Update(string name, decimal percentage)
    {
        Name = name;
        Percentage = percentage;
    }

}



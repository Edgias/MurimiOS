namespace Edgias.MurimiOS.Domain.Entities;

public class Loan(string name, string description, int duration, decimal principalAmount,
        decimal interestRate, decimal effectiveRate, Guid currencyId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Description { get; private set; } = description;

    public int Duration { get; private set; } = duration;

    public decimal PrincipalAmount { get; private set; } = principalAmount;

    public decimal InterestRate { get; private set; } = interestRate;

    public decimal EffectiveRate { get; private set; } = effectiveRate;

    public Guid CurrencyId { get; private set; } = currencyId;

    public Currency Currency { get; private set; } = default!;

    public void Update(string name, string description, int duration,
        decimal principalAmount, decimal interestRate, decimal effectiveRate)
    {
        Name = name;
        Description = description;
        Duration = duration;
        PrincipalAmount = principalAmount;
        InterestRate = interestRate;
        EffectiveRate = effectiveRate;
    }

}



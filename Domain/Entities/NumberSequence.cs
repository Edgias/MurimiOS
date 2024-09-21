namespace Edgias.MurimiOS.Domain.Entities;

public class NumberSequence(string entity, string prefix, string seperator,
        int startingNumber, string suffix) : BaseEntity, IAggregateRoot
{
    public string Entity { get; private set; } = entity;

    public string Prefix { get; private set; } = prefix;

    public string Seperator { get; private set; } = seperator;

    public int StartingNumber { get; private set; } = startingNumber;

    public string Suffix { get; private set; } = suffix;

    public void Update(string prefix, string seperator, string suffix)
    {
        Prefix = prefix;
        Seperator = seperator;
        Suffix = suffix;
    }

    public string GenerateSequence(int count)
    {
        string number;

        if (count == 0)
        {
            number = StartingNumber < 10 ? $"0{StartingNumber}" : $"{StartingNumber}";
        }
        else
        {
            number = count < 10 ? $"0{count + 1}" : $"{count + 1}";
        }

        string numberSequence = string.IsNullOrEmpty(Suffix) ? $"{Prefix}{Seperator}{number}" :
            $"{Prefix}{Seperator}{number}{Seperator}{Suffix}";

        return numberSequence;
    }
}



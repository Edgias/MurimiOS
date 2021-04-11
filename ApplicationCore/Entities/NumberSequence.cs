using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class NumberSequence : BaseEntity, IAggregateRoot
    {
        public string Entity { get; private set; }

        public string Prefix { get; private set; }

        public string Seperator { get; private set; }

        public int StartingNumber { get; private set; }

        public string Suffix { get; private set; }

        public NumberSequence(string entity, string prefix, string seperator, int startingNumber, string suffix)
        {
            Guard.AgainstNullOrEmpty(entity, nameof(entity));
            Guard.AgainstNullOrEmpty(prefix, nameof(prefix));
            Guard.AgainstNullOrEmpty(seperator, nameof(seperator));
            Guard.AgainstZero(startingNumber, nameof(startingNumber));

            Entity = entity;
            Prefix = prefix;
            Seperator = seperator;
            StartingNumber = startingNumber;
            Suffix = suffix;
        }

        public void UpdateDetails(string prefix, string seperator, string suffix)
        {
            Guard.AgainstNullOrEmpty(prefix, nameof(prefix));
            Guard.AgainstNullOrEmpty(seperator, nameof(seperator));

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
}

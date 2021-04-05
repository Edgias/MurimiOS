namespace Murimi.ApplicationCore.Entities
{
    public class NumberSequence : BaseEntity
    {
        public string Entity { get; set; }

        public string Prefix { get; set; }

        public string Seperator { get; set; }

        public int StartingNumber { get; set; }

        public string Suffix { get; set; }

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

            string numberSequence = string.IsNullOrEmpty(Suffix) ? $"{Prefix}{Seperator}{number}" : $"{Prefix}{Seperator}{number}{Seperator}{Suffix}";

            return numberSequence;
        }
    }
}

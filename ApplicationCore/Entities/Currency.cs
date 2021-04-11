using Murimi.ApplicationCore.SharedKernel;

namespace Murimi.ApplicationCore.Entities
{
    public class Currency : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Symbol { get; private set; }

        public Currency(string name, string symbol)
        {
            SetData(name, symbol);
        }

        public void UpdateDetails(string name, string symbol)
        {
            SetData(name, symbol);
        }

        private void SetData(string name, string symbol)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(symbol, nameof(symbol));

            Name = name;
            Symbol = symbol;
        }
    }
}

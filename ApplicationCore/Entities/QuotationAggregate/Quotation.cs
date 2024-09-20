using Edgias.MurimiOS.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace Edgias.MurimiOS.Domain.Entities.QuotationAggregate
{
    public class Quotation : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public DateTimeOffset QuotationDate { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ValidUntil { get; private set; }

        public Guid? CustomerId { get; private set; }

        public Customer Customer { get; set; }

        private readonly List<QuotationItem> _quotationItems = new();

        public IReadOnlyCollection<QuotationItem> QuotationItems => _quotationItems.AsReadOnly();

        private Quotation()
        {
            // Required by EF
        }

        public Quotation(string name, DateTimeOffset quotationDate, DateTimeOffset validUntil, Guid? customerId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(quotationDate, nameof(quotationDate));
            Guard.AgainstNull(validUntil, nameof(validUntil));

            Name = name;
            QuotationDate = quotationDate;
            ValidUntil = validUntil;
            CustomerId = customerId;
        }

        public Quotation(string name, DateTimeOffset quotationDate, DateTimeOffset validUntil, List<QuotationItem> quotationItems, Guid? customerId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(quotationDate, nameof(quotationDate));
            Guard.AgainstNull(validUntil, nameof(validUntil));
            Guard.AgainstNull(quotationItems, nameof(quotationItems));

            Name = name;
            QuotationDate = quotationDate;
            ValidUntil = validUntil;
            CustomerId = customerId;
            _quotationItems = quotationItems;
        }

        public void AddItem(QuotationItem quotationItem)
        {
            Guard.AgainstNull(quotationItem, nameof(quotationItem));

            _quotationItems.Add(quotationItem);
        }

        public decimal Total()
        {
            decimal total = 0m;

            foreach (QuotationItem quotationItem in _quotationItems)
            {
                decimal lineTotalBeforeTax = quotationItem.UnitPrice * quotationItem.Units;
                decimal lineTotalAfterTax = quotationItem.TaxId != null ? (lineTotalBeforeTax + (lineTotalBeforeTax * (quotationItem.Tax.Percentage / 100))) : lineTotalBeforeTax;
                total += lineTotalAfterTax;
            }

            return total;
        }

        public decimal TotalTax()
        {
            decimal total = 0m;

            foreach (QuotationItem quotationItem in _quotationItems)
            {
                decimal lineTotalBeforeTax = quotationItem.UnitPrice * quotationItem.Units;
                decimal lineTax = quotationItem.TaxId != null ? (lineTotalBeforeTax * (quotationItem.Tax.Percentage / 100)) : 0;
                total += lineTax;
            }

            return total;
        }

        public decimal TotalWithoutTax()
        {
            decimal total = 0m;

            foreach (QuotationItem quotationItem in _quotationItems)
            {
                decimal lineTotal = quotationItem.UnitPrice * quotationItem.Units;
                total += lineTotal;
            }

            return total;
        }

        public void ChangeValidDate(DateTimeOffset validUntil)
        {
            ValidUntil = validUntil;
        }

    }
}

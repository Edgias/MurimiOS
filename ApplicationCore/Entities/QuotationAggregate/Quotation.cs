using System;
using System.Collections.Generic;

namespace Murimi.ApplicationCore.Entities.QuotationAggregate
{
    public class Quotation : BaseEntity
    {
        public string Name { get; private set; }

        public DateTime QuotationDate { get; private set; }

        public DateTime ValidUntil { get; private set; }

        public Guid? CustomerId { get; private set; }

        public Customer Customer { get; set; }

        private readonly List<QuotationItem> _quotationItems = new List<QuotationItem>();

        public IReadOnlyCollection<QuotationItem> QuotationItems => _quotationItems.AsReadOnly();

        private Quotation()
        {
        }

        public Quotation(string name, DateTime quotationDate, DateTime validUntil, Guid? customerId)
        {
            Name = name;
            QuotationDate = quotationDate;
            ValidUntil = validUntil;
            CustomerId = customerId;
        }

        public Quotation(string name, DateTime quotationDate, DateTime validUntil, List<QuotationItem> quotationItems, Guid? customerId)
        {
            Name = name;
            QuotationDate = quotationDate;
            ValidUntil = validUntil;
            CustomerId = customerId;
            _quotationItems = quotationItems;
        }

        public void AddItem(QuotationItem quotationItem)
        {
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

        public void ChangeValidDate(DateTime validUntil)
        {
            ValidUntil = validUntil;
        }

    }
}

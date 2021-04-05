using Murimi.ApplicationCore.Entities.SalesOrderAggregate;
using System;
using System.Collections.Generic;

namespace Murimi.ApplicationCore.Entities.SalesInvoiceAggregate
{
    public class SalesInvoice : BaseEntity
    {
        public string Name { get; private set; }

        public DateTimeOffset InvoiceDate { get; private set; } = DateTimeOffset.Now;

        public DateTimeOffset DueDate { get; private set; }

        public Address BillingAddress { get; private set; }

        public string InvoiceNotes { get; private set; }

        public Guid? SalesOrderId { get; private set; }

        public SalesOrder SalesOrder { get; private set; }

        public Guid? CustomerId { get; private set; }

        public Customer Customer { get; private set; }

        
        private readonly List<SalesInvoiceItem> _salesInvoiceItems = new List<SalesInvoiceItem>();

        public IReadOnlyCollection<SalesInvoiceItem> InvoiceItems => _salesInvoiceItems.AsReadOnly();

        private SalesInvoice()
        {
        }

        public SalesInvoice(string name, DateTimeOffset dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId, Address billingAddress)
        {
            Name = name;
            BillingAddress = billingAddress;
            InvoiceNotes = invoiceNotes;
            DueDate = dueDate;
            SalesOrderId = salesOrderId;
            CustomerId = customerId;
        }

        public SalesInvoice(string name, DateTimeOffset dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId, Address billingAddress, List<SalesInvoiceItem> salesInvoiceItems)
        {
            Name = name;
            BillingAddress = billingAddress;
            InvoiceNotes = invoiceNotes;
            DueDate = dueDate;
            SalesOrderId = salesOrderId;
            CustomerId = customerId;
            _salesInvoiceItems = salesInvoiceItems;
        }

        public void AddItem(SalesInvoiceItem salesInvoiceItem)
        {
            _salesInvoiceItems.Add(salesInvoiceItem);
        }

        public decimal Total()
        {
            decimal total = 0m;

            foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
            {
                decimal lineTotalBeforeTax = invoiceLine.UnitPrice * invoiceLine.Units;
                decimal lineTotalAfterTax = invoiceLine.TaxId != null ? (lineTotalBeforeTax + (lineTotalBeforeTax * (invoiceLine.Tax.Percentage / 100))) : lineTotalBeforeTax;
                total += lineTotalAfterTax;
            }

            return total;
        }

        public decimal TotalTax()
        {
            decimal total = 0m;

            foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
            {
                decimal lineTotalBeforeTax = invoiceLine.UnitPrice * invoiceLine.Units;
                decimal lineTax = invoiceLine.TaxId != null ? (lineTotalBeforeTax * (invoiceLine.Tax.Percentage / 100)) : 0;
                total += lineTax;
            }

            return total;
        }

        public decimal TotalWithoutTax()
        {
            decimal total = 0m;

            foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
            {
                decimal lineTotal = invoiceLine.UnitPrice * invoiceLine.Units;
                total += lineTotal;
            }

            return total;
        }

    }
}

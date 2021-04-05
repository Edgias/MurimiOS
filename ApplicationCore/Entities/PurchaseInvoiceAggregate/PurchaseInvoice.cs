using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using System;
using System.Collections.Generic;

namespace Edgias.Agrik.ApplicationCore.Entities.PurchaseInvoiceAggregate
{
    public class PurchaseInvoice : BaseEntity
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

        private readonly List<PurchaseInvoiceItem> _purchaseInvoiceItems = new List<PurchaseInvoiceItem>();

        public IReadOnlyCollection<PurchaseInvoiceItem> PurchaseInvoiceItems => _purchaseInvoiceItems.AsReadOnly();

        private PurchaseInvoice()
        {
        }

        public PurchaseInvoice(string name, DateTimeOffset dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId, Address billingAddress)
        {
            Name = name;
            BillingAddress = billingAddress;
            InvoiceNotes = invoiceNotes;
            DueDate = dueDate;
            SalesOrderId = salesOrderId;
            CustomerId = customerId;
        }

        public PurchaseInvoice(string name, DateTimeOffset dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId, Address billingAddress, List<PurchaseInvoiceItem> purchaseInvoiceItems)
        {
            Name = name;
            BillingAddress = billingAddress;
            InvoiceNotes = invoiceNotes;
            DueDate = dueDate;
            SalesOrderId = salesOrderId;
            CustomerId = customerId;
            _purchaseInvoiceItems = purchaseInvoiceItems;
        }

        public void AddItem(PurchaseInvoiceItem purchaseInvoiceItem)
        {
            _purchaseInvoiceItems.Add(purchaseInvoiceItem);
        }

        public decimal Total()
        {
            decimal total = 0m;

            foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
            {
                decimal lineTotalBeforeTax = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
                decimal lineTotalAfterTax = purchaseInvoiceItem.TaxId != null ? (lineTotalBeforeTax + (lineTotalBeforeTax * (purchaseInvoiceItem.Tax.Percentage / 100))) : lineTotalBeforeTax;
                total += lineTotalAfterTax;
            }

            return total;
        }

        public decimal TotalTax()
        {
            decimal total = 0m;

            foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
            {
                decimal lineTotalBeforeTax = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
                decimal lineTax = purchaseInvoiceItem.TaxId != null ? (lineTotalBeforeTax * (purchaseInvoiceItem.Tax.Percentage / 100)) : 0;
                total += lineTax;
            }

            return total;
        }

        public decimal TotalWithoutTax()
        {
            decimal total = 0m;

            foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
            {
                decimal lineTotal = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
                total += lineTotal;
            }

            return total;
        }

    }
}

using Murimi.ApplicationCore.Entities.PurchaseInvoiceAggregate;
using Murimi.ApplicationCore.SharedKernel;
using System;

namespace Murimi.ApplicationCore.Entities
{
    public class Asset : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string SerialNumber { get; private set; }

        public string Description { get; private set; }

        public DateTimeOffset? PurchaseDate { get; private set; }

        public decimal? PurchasePrice { get; private set; }

        public Guid? SupplierId { get; private set; }

        public Supplier Supplier { get; private set; }

        public Guid? PurchaseInvoiceId { get; private set; }

        public PurchaseInvoice PurchaseInvoice { get; private set; }

        public Asset(string name, string serialNumber, string description, DateTimeOffset purchaseDate, decimal? purchasePrice, Guid? supplierId, Guid? purchaseInvoiceId)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(serialNumber, nameof(serialNumber));

            Name = name;
            SerialNumber = serialNumber;
            Description = description;
            PurchaseDate = purchaseDate;
            PurchasePrice = purchasePrice;
            SupplierId = supplierId;
            PurchaseInvoiceId = purchaseInvoiceId;
        }

        public void UpdateDetails(string name, string serialNumber, string description, DateTimeOffset purchaseDate, decimal? purchasePrice)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNullOrEmpty(serialNumber, nameof(serialNumber));

            Name = name;
            Description = description;
            SerialNumber = serialNumber;
            PurchaseDate = purchaseDate;
            PurchasePrice = purchasePrice;
        }

        public void UpdateSupplier(Guid supplierId)
        {
            Guard.AgainstNull(supplierId, nameof(supplierId));

            SupplierId = supplierId;
        }

        public void UpdateInvoice(Guid invoiceId)
        {
            Guard.AgainstNull(invoiceId, nameof(invoiceId));

            PurchaseInvoiceId = invoiceId;
        }
    }
}

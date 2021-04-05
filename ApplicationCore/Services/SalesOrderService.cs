using Edgias.Agrik.ApplicationCore.Entities;
using Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate;
using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using Edgias.Agrik.ApplicationCore.Interfaces;
using Edgias.Agrik.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edgias.Agrik.ApplicationCore.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IAsyncRepository<NumberSequence> _numberSequenceRepository;
        private readonly IAsyncRepository<PriceList> _priceListRepository;
        private readonly IAsyncRepository<Item> _itemRepository;
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IAsyncRepository<SalesInvoiceNote> _salesInvoiceNoteRepository;
        private readonly IAsyncRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IAsyncRepository<SalesOrder> _salesOrderRepository;

        public SalesOrderService(IAsyncRepository<NumberSequence> numberSequenceRepository,
            IAsyncRepository<PriceList> priceListRepository,
            IAsyncRepository<Item> itemRepository,
            IAsyncRepository<Customer> customerRepository,
            IAsyncRepository<SalesInvoiceNote> invoiceNoteRepository,
            IAsyncRepository<SalesInvoice> salesInvoiceRepository,
            IAsyncRepository<SalesOrder> salesOrderRepository)
        {
            _numberSequenceRepository = numberSequenceRepository;
            _priceListRepository = priceListRepository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _salesInvoiceNoteRepository = invoiceNoteRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _salesOrderRepository = salesOrderRepository;
        }

        public async Task AddSalesOrderItemAsync(Guid salesOrderId, Guid itemId, Guid priceListId, string userId, int units = 1)
        {
            SalesOrder salesOrder = await _salesOrderRepository.GetByIdAsync(salesOrderId);

            Item item = await _itemRepository.GetByIdAsync(itemId);
            ItemOrdered itemQuoted = new ItemOrdered(item.Id, item.Name);

            PriceList priceList = await _priceListRepository.GetByIdAsync(priceListId);
            SalesOrderItem salesOrderItem = new SalesOrderItem(itemQuoted, priceList.Amount, units)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };
            salesOrder.AddItem(salesOrderItem);

            await _salesOrderRepository.UpdateAsync(salesOrder);
        }

        public async Task ConvertToInvoiceAsync(Guid salesOrderId, string userId)
        {
            SalesOrder salesOrder = await _salesOrderRepository.GetSingleBySpecificationAsync(new SalesOrderSpecification(salesOrderId));

            Address billingAddress;
            Customer customer = await _customerRepository.GetSingleBySpecificationAsync(new CustomerSpecification(salesOrder.CustomerId.Value));
            billingAddress = customer.BillingAddress;

            List<SalesInvoiceItem> invoiceLines = salesOrder.SalesOrderItems.Select(salesOrderLine =>
            {
                InvoicedItem invoicedItem = new InvoicedItem(salesOrderLine.ItemOrdered.ItemId, salesOrderLine.ItemOrdered.ItemName, "");
                SalesInvoiceItem invoiceLine = new SalesInvoiceItem(invoicedItem, salesOrderLine.UnitPrice, salesOrderLine.Units, null)
                {
                    CreatedBy = userId,
                    LastModifiedBy = userId
                };
                return invoiceLine;
            }).ToList();

            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(SalesInvoice).Name));

            int invoiceCount = await _salesInvoiceRepository.CountAllAsync();
            string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

            SalesInvoiceNote invoiceNote = await _salesInvoiceNoteRepository.GetSingleBySpecificationAsync(new SalesInvoiceNoteSpecification());

            SalesInvoice invoice = new SalesInvoice(invoiceNumber, DateTime.Now, invoiceNote?.Description, salesOrder.CustomerId, salesOrderId, billingAddress, invoiceLines)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            await _salesInvoiceRepository.AddAsync(invoice);
        }

        public async Task<SalesOrder> CreateSalesOrderAsync(Guid? customerId, Guid? quotationId, string userId)
        {
            string salesOrderName = typeof(SalesOrder).Name;
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(salesOrderName));

            int salesOrderCount = await _salesOrderRepository.CountAllAsync();
            string salesOrderNumber = numberSequence.GenerateSequence(salesOrderCount);

            Address shipToAddress = null;

            if (customerId.HasValue)
            {
                Customer customer = await _customerRepository.GetByIdAsync(customerId.Value);
                shipToAddress = customer.BillingAddress;
            }

            if(shipToAddress == null)
            {
                throw new Exception("Shipping address cannot be null.");
            }

            SalesOrder salesOrder = new SalesOrder(salesOrderNumber, customerId, quotationId, shipToAddress)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            salesOrder = await _salesOrderRepository.AddAsync(salesOrder);

            return salesOrder;
        }
    }
}

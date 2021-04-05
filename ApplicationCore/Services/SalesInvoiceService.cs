using Edgias.Agrik.ApplicationCore.Entities;
using Edgias.Agrik.ApplicationCore.Entities.SalesInvoiceAggregate;
using Edgias.Agrik.ApplicationCore.Interfaces;
using Edgias.Agrik.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edgias.Agrik.ApplicationCore.Services
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly IAsyncRepository<NumberSequence> _numberSequenceRepository;
        private readonly IAsyncRepository<PriceList> _priceListRepository;
        private readonly IAsyncRepository<Item> _itemRepository;
        private readonly IAsyncRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IAsyncRepository<SalesInvoiceNote> _salesInvoiceNoteRepository;

        public SalesInvoiceService(IAsyncRepository<NumberSequence> numberSequenceRepository,
            IAsyncRepository<PriceList> priceListRepository,
            IAsyncRepository<Item> itemRepository,
            IAsyncRepository<SalesInvoice> salesInvoiceRepository,
            IAsyncRepository<Customer> customerRepository,
            IAsyncRepository<SalesInvoiceNote> salesInvoiceNoteRepository)
        {
            _numberSequenceRepository = numberSequenceRepository;
            _priceListRepository = priceListRepository;
            _itemRepository = itemRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _customerRepository = customerRepository;
            _salesInvoiceNoteRepository = salesInvoiceNoteRepository;
        }

        public async Task AddInvoiceItemAsync(Guid invoiceId, Guid itemId, Guid priceListId, Guid? taxId, string userId, int units = 1)
        {
            SalesInvoice salesInvoice = await _salesInvoiceRepository.GetByIdAsync(invoiceId);

            Item item = await _itemRepository.GetByIdAsync(itemId);
            InvoicedItem invoicedItem = new InvoicedItem(item.Id, item.Name, item.Description);

            PriceList priceList = await _priceListRepository.GetByIdAsync(priceListId);
            SalesInvoiceItem salesInvoiceItem = new SalesInvoiceItem(invoicedItem, priceList.Amount, units, taxId)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            salesInvoice.AddItem(salesInvoiceItem);

            await _salesInvoiceRepository.UpdateAsync(salesInvoice);
        }

        public async Task<SalesInvoice> CreateInvoiceAsync(DateTimeOffset dueDate, Guid? invoiceNoteId, Guid? customerId, Guid? salesOrderId, string userId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(SalesInvoice).Name));

            int invoiceCount = await _salesInvoiceRepository.CountAllAsync();

            string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

            Address billingAddress;
            Customer contact = await _customerRepository.GetSingleBySpecificationAsync(new CustomerSpecification(customerId.Value));
            billingAddress = contact.BillingAddress;

            string invoiceNotes = string.Empty;

            if(invoiceNoteId.HasValue)
            {
                SalesInvoiceNote salesInvoiceNote = await _salesInvoiceNoteRepository.GetByIdAsync(invoiceNoteId.Value);
                invoiceNotes = salesInvoiceNote?.Description;
            }

            SalesInvoice salesInvoice = new SalesInvoice(invoiceNumber, dueDate, invoiceNotes, customerId, salesOrderId, billingAddress)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            salesInvoice = await _salesInvoiceRepository.AddAsync(salesInvoice);

            return salesInvoice;
        }

        public async Task<SalesInvoice> CreateInvoiceAsync(DateTimeOffset dueDate, Guid? invoiceNoteId, List<SalesInvoiceItem> invoiceItems, 
            Guid? customerId, Guid? salesOrderId, string userId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(SalesInvoice).Name));

            int invoiceCount = await _salesInvoiceRepository.CountAllAsync();

            string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

            Address billingAddress;
            Customer customer = await _customerRepository.GetSingleBySpecificationAsync(new CustomerSpecification(customerId.Value));
            billingAddress = customer.BillingAddress;

            string invoiceNotes = string.Empty;

            if (invoiceNoteId.HasValue)
            {
                SalesInvoiceNote salesInvoiceNote = await _salesInvoiceNoteRepository.GetByIdAsync(invoiceNoteId.Value);
                invoiceNotes = salesInvoiceNote?.Description;
            }

            SalesInvoice invoice = new SalesInvoice(invoiceNumber, dueDate, invoiceNotes, customerId, salesOrderId, billingAddress, invoiceItems)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            invoice = await _salesInvoiceRepository.AddAsync(invoice);

            return invoice;
        }
    }
}

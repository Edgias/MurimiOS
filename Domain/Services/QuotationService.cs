using Edgias.MurimiOS.Domain.Entities;
using Edgias.MurimiOS.Domain.Entities.QuotationAggregate;
using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;
using Edgias.MurimiOS.Domain.Interfaces;
using Edgias.MurimiOS.Domain.SharedKernel;
using Edgias.MurimiOS.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IAsyncRepository<NumberSequence> _numberSequenceRepository;
        private readonly IAsyncRepository<PriceList> _priceListRepository;
        private readonly IAsyncRepository<Item> _itemRepository;
        private readonly IAsyncRepository<SalesOrder> _salesOrderRepository;
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IAsyncRepository<Quotation> _quotationRepository;

        public QuotationService(IAsyncRepository<NumberSequence> numberSequenceRepository,
            IAsyncRepository<PriceList> priceListRepository,
            IAsyncRepository<Item> itemRepository,
            IAsyncRepository<SalesOrder> salesOrderRepository,
            IAsyncRepository<Customer> customerRepository,
            IAsyncRepository<Quotation> quotationRepository)
        {
            _numberSequenceRepository = numberSequenceRepository;
            _priceListRepository = priceListRepository;
            _itemRepository = itemRepository;
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _quotationRepository = quotationRepository;
        }

        public async Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1)
        {
            Guard.AgainstNull(quotationId, nameof(quotationId));
            Guard.AgainstNull(priceListId, nameof(priceListId));
            Guard.AgainstNull(itemId, nameof(itemId));

            Quotation quotation = await _quotationRepository.GetByIdAsync(quotationId);

            Item item = await _itemRepository.GetByIdAsync(itemId);
            ItemQuoted itemQuoted = new(item.Id, item.Name, item.Description);

            PriceList priceList = await _priceListRepository.GetByIdAsync(priceListId);

            QuotationItem quotationItem = new(itemQuoted, priceList.UnitPrice, units, taxId);

            quotation.AddItem(quotationItem);

            await _quotationRepository.UpdateAsync(quotation);
        }

        public async Task ConvertToSalesOrderAsync(Guid quotationId)
        {
            Quotation quotation = await _quotationRepository.GetSingleBySpecificationAsync(new QuotationSpecification(quotationId));

            Address shipToAddress;
            Customer customer = await _customerRepository.GetSingleBySpecificationAsync(new CustomerSpecification(quotation.CustomerId.Value));
            shipToAddress = customer.BillingAddress;

            List<SalesOrderItem> salesOrderItems = quotation.QuotationItems.Select(quotationItem =>
            {
                ItemOrdered itemOrdered = new(quotationItem.ItemQuoted.ItemId, quotationItem.ItemQuoted.ItemName, quotationItem.ItemQuoted.ItemDescription);
                SalesOrderItem salesOrderItem = new(itemOrdered, quotationItem.UnitPrice, quotationItem.Units);
                return salesOrderItem;
            }).ToList();

            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(SalesOrder).Name));

            int salesOrderCount = await _salesOrderRepository.CountAllAsync();
            string salesOrderNumber = numberSequence.GenerateSequence(salesOrderCount);

            SalesOrder salesOrder = new(salesOrderNumber, quotation.CustomerId, quotationId, shipToAddress, salesOrderItems);

            await _salesOrderRepository.AddAsync(salesOrder);
        }

        public async Task<Quotation> CreateQuotationAsync(DateTimeOffset quotationDate, DateTimeOffset validUntil, Guid? customerId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(Quotation).Name));

            int quotationCount = await _quotationRepository.CountAllAsync();
            string quotationNumber = numberSequence.GenerateSequence(quotationCount);

            Quotation quotation = new Quotation(quotationNumber, quotationDate, validUntil, customerId);

            quotation = await _quotationRepository.AddAsync(quotation);

            return quotation;
        }

        public async Task<Quotation> CreateQuotationAsync(DateTimeOffset quotationDate, DateTimeOffset validUntil, List<QuotationItem> quotationItems, Guid? customerId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(Quotation).Name));

            int quotationCount = await _quotationRepository.CountAllAsync();
            string quotationNumber = numberSequence.GenerateSequence(quotationCount);

            Quotation quotation = new(quotationNumber, quotationDate, validUntil, quotationItems, customerId);

            quotation = await _quotationRepository.AddAsync(quotation);

            return quotation;
        }
    }
}

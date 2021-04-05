using Edgias.Agrik.ApplicationCore.Entities;
using Edgias.Agrik.ApplicationCore.Entities.QuotationAggregate;
using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using Edgias.Agrik.ApplicationCore.Interfaces;
using Edgias.Agrik.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edgias.Agrik.ApplicationCore.Services
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
            IAsyncRepository<Customer> customerRepository)
        {
            _numberSequenceRepository = numberSequenceRepository;
            _priceListRepository = priceListRepository;
            _itemRepository = itemRepository;
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
        }

        public async Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, string userId, int units = 1)
        {
            Quotation quotation = await _quotationRepository.GetByIdAsync(quotationId);

            Item item = await _itemRepository.GetByIdAsync(itemId);
            ItemQuoted itemQuoted = new ItemQuoted(item.Id, item.Name, item.Description);

            PriceList priceList = await _priceListRepository.GetByIdAsync(priceListId);

            QuotationItem quotationItem = new QuotationItem(itemQuoted, priceList.Amount, units, taxId)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            quotation.AddItem(quotationItem);

            await _quotationRepository.UpdateAsync(quotation);
        }

        public async Task ConvertToSalesOrderAsync(Guid quotationId, string userId)
        {
            Quotation quotation = await _quotationRepository.GetSingleBySpecificationAsync(new QuotationSpecification(quotationId));

            Address shipToAddress;
            Customer customer = await _customerRepository.GetSingleBySpecificationAsync(new CustomerSpecification(quotation.CustomerId.Value));
            shipToAddress = customer.BillingAddress;

            List<SalesOrderItem> salesOrderItems = quotation.QuotationItems.Select(quotationItem =>
            {
                ItemOrdered itemOrdered = new ItemOrdered(quotationItem.ItemQuoted.ItemId, quotationItem.ItemQuoted.ItemName);
                SalesOrderItem salesOrderItem = new SalesOrderItem(itemOrdered, quotationItem.UnitPrice, quotationItem.Units)
                {
                    CreatedBy = userId,
                    LastModifiedBy = userId
                };
                return salesOrderItem;
            }).ToList();

            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(SalesOrder).Name));

            int salesOrderCount = await _salesOrderRepository.CountAllAsync();
            string salesOrderNumber = numberSequence.GenerateSequence(salesOrderCount);

            SalesOrder salesOrder = new SalesOrder(salesOrderNumber, quotation.CustomerId, quotationId, shipToAddress, salesOrderItems)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            await _salesOrderRepository.AddAsync(salesOrder);
        }

        public async Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime validUntil, Guid? customerId, string userId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(Quotation).Name));

            int quotationCount = await _quotationRepository.CountAllAsync();
            string quotationNumber = numberSequence.GenerateSequence(quotationCount);

            Quotation quotation = new Quotation(quotationNumber, quotationDate, validUntil, customerId)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            quotation = await _quotationRepository.AddAsync(quotation);

            return quotation;
        }

        public async Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime validUntil, List<QuotationItem> quotationItems, Guid? customerId, string userId)
        {
            NumberSequence numberSequence = await _numberSequenceRepository.GetSingleBySpecificationAsync(new NumberSequenceSpecification(typeof(Quotation).Name));

            int quotationCount = await _quotationRepository.CountAllAsync();
            string quotationNumber = numberSequence.GenerateSequence(quotationCount);

            Quotation quotation = new Quotation(quotationNumber, quotationDate, validUntil, quotationItems, customerId)
            {
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            quotation = await _quotationRepository.AddAsync(quotation);

            return quotation;
        }
    }
}

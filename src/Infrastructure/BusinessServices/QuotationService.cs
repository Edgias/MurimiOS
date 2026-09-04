using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;

namespace EdgIQ.MurimiOS.Infrastructure.BusinessServices;
using Address = global::EdgIQ.MurimiOS.Domain.Common.Address;
using Guard = global::EdgIQ.MurimiOS.Domain.Common.Guard;
using Customer = global::EdgIQ.MurimiOS.Domain.Customer.Customer;
using Item = global::EdgIQ.MurimiOS.Domain.Item.Item;
using NumberSequence = global::EdgIQ.MurimiOS.Domain.NumberSequence.NumberSequence;
using PriceList = global::EdgIQ.MurimiOS.Domain.PriceList.PriceList;
using ItemQuoted = global::EdgIQ.MurimiOS.Domain.Quotation.ItemQuoted;
using Quotation = global::EdgIQ.MurimiOS.Domain.Quotation.Quotation;
using QuotationItem = global::EdgIQ.MurimiOS.Domain.Quotation.QuotationItem;
using ItemOrdered = global::EdgIQ.MurimiOS.Domain.SalesOrder.ItemOrdered;
using SalesOrder = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrder;
using SalesOrderItem = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrderItem;

using IQuotationService = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IQuotationService;
using MurimiOSDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.MurimiOSDbContext;

public class QuotationService(MurimiOSDbContext dbContext) : IQuotationService
{
    public async Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1)
    {
        Guard.AgainstNull(quotationId, nameof(quotationId));
        Guard.AgainstNull(priceListId, nameof(priceListId));
        Guard.AgainstNull(itemId, nameof(itemId));

        Quotation? quotation = await dbContext.Quotations.FindAsync(quotationId) ?? throw new ArgumentNullException($"Quotation with id {quotationId} cannot be found.");

        Item? item = await dbContext.Items.FindAsync(itemId) ?? throw new ArgumentNullException($"Item with id {itemId} cannot be found.");
        
        ItemQuoted itemQuoted = new(item.Id, item.Name, item.Description);

        PriceList? priceList = await dbContext.PriceLists.FindAsync(priceListId) ?? throw new ArgumentNullException($"Price list with id {priceListId} cannot be found.");

        QuotationItem quotationItem = new(itemQuoted, priceList.UnitPrice, units, taxId);

        quotation.AddItem(quotationItem);

        dbContext.Update(quotation);
        await dbContext.SaveChangesAsync();
    }

    public async Task ConvertToSalesOrderAsync(Guid quotationId)
    {
        Quotation? quotation = await dbContext.Quotations.
            Include(q => q.QuotationItems).
            ThenInclude(qi => qi.ItemQuoted).FirstOrDefaultAsync(q => q.Id == quotationId) ?? 
            throw new ArgumentNullException($"Quotation with id {quotationId} cannot be found.");

        List<SalesOrderItem> salesOrderItems = quotation.QuotationItems.Select(quotationItem =>
        {
            ItemOrdered itemOrdered = new(quotationItem.ItemQuoted.ItemId, quotationItem.ItemQuoted.ItemName, quotationItem.ItemQuoted.ItemDescription);
            SalesOrderItem salesOrderItem = new(itemOrdered, quotationItem.UnitPrice, quotationItem.Units);
            return salesOrderItem;
        }).ToList();

        NumberSequence? numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(SalesOrder).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(SalesOrder).Name} not configured.");

        int salesOrderCount = await dbContext.SalesOrders.CountAsync();
        string salesOrderNumber = numberSequence.GenerateSequence(salesOrderCount);

        // Get customer's shipping address
        Address shipToAddress;
        Customer? customer = await dbContext.Customers.FindAsync(quotation.CustomerId!.Value) ??
            throw new ArgumentNullException($"Customer with id {quotation.CustomerId!.Value} cannot be found.");

        shipToAddress = customer.BillingAddress;

        SalesOrder salesOrder = new(salesOrderNumber, quotation.CustomerId, quotationId, shipToAddress, salesOrderItems);

        dbContext.Add(salesOrder);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, Guid? customerId)
    {
        NumberSequence numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(Quotation).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(Quotation).Name} not configured.");

        int quotationCount = await dbContext.Quotations.CountAsync();
        string quotationNumber = numberSequence.GenerateSequence(quotationCount);

        Quotation quotation = new(quotationNumber, quotationDate, expiryDate, customerId);

        quotation = dbContext.Quotations.Add(quotation).Entity;
        await dbContext.SaveChangesAsync();

        return quotation;
    }

    public async Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime expiryDate, List<QuotationItem> quotationItems, Guid? customerId)
    {
        NumberSequence numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(Quotation).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(Quotation).Name} not configured.");

        int quotationCount = await dbContext.Quotations.CountAsync();
        string quotationNumber = numberSequence.GenerateSequence(quotationCount);

        Quotation quotation = new(quotationNumber, quotationDate, expiryDate, quotationItems, customerId);

        quotation = dbContext.Quotations.Add(quotation).Entity;
        await dbContext.SaveChangesAsync();

        return quotation;
    }

    
}



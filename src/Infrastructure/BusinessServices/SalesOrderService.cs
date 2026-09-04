using Edgias.MurimiOS.Domain.Interfaces;
using Edgias.MurimiOS.Infrastructure.Data;

namespace Edgias.MurimiOS.Infrastructure.Services;

public class SalesOrderService(MurimiOSDbContext dbContext) : ISalesOrderService
{
    public async Task AddSalesOrderItemAsync(Guid salesOrderId, Guid itemId, Guid priceListId, int units = 1)
    {
        SalesOrder salesOrder = await dbContext.SalesOrders.FindAsync(salesOrderId) ??
            throw new ArgumentNullException($"Sales Order with id {salesOrderId} cannot be found.");

        Item? item = await dbContext.Items.FindAsync(itemId) ?? throw new ArgumentNullException($"Item with id {itemId} cannot be found.");

        ItemOrdered itemOrdered = new(item.Id, item.Name, item.Description);

        PriceList? priceList = await dbContext.PriceLists.FindAsync(priceListId) ?? throw new ArgumentNullException($"Price list with id {priceListId} cannot be found.");
        SalesOrderItem salesOrderItem = new(itemOrdered, priceList.UnitPrice, units);
        salesOrder.AddItem(salesOrderItem);

        dbContext.SalesOrders.Add(salesOrder);
        await dbContext.SaveChangesAsync();
    }

    public async Task ConvertToInvoiceAsync(Guid salesOrderId)
    {
        SalesOrder? salesOrder = await dbContext.SalesOrders.
        Include(so => so.SalesOrderItems).
            ThenInclude(soi => soi.ItemOrdered).FirstOrDefaultAsync(so => so.Id == salesOrderId) ??
            throw new ArgumentNullException($"Sales Order with id {salesOrderId} cannot be found.");

        List<SalesInvoiceItem> invoiceLines = salesOrder.SalesOrderItems.Select(salesOrderLine =>
        {
            Domain.Entities.SalesInvoiceAggregate.InvoicedItem invoicedItem = new(salesOrderLine.ItemOrdered.ItemId, salesOrderLine.ItemOrdered.ItemName, salesOrderLine.ItemOrdered.ItemDescription);
            SalesInvoiceItem invoiceLine = new(invoicedItem, salesOrderLine.UnitPrice, salesOrderLine.Units, null);
            return invoiceLine;
        }).ToList();

        NumberSequence? numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(SalesInvoice).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(SalesInvoice).Name} not configured.");

        int invoiceCount = await dbContext.SalesInvoices.CountAsync();
        string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

        SalesInvoiceNote? invoiceNote = await dbContext.SalesInvoiceNotes.FirstOrDefaultAsync();

        // Get customer's billing address
        Address billingAddress;
        Customer? customer = await dbContext.Customers.FindAsync(salesOrder.CustomerId!.Value) ??
            throw new ArgumentNullException($"Customer with id {salesOrder.CustomerId!.Value} cannot be found.");
        billingAddress = customer.BillingAddress;

        SalesInvoice invoice = new(invoiceNumber, DateTime.Now, invoiceNote?.Description!, salesOrder.CustomerId, salesOrderId, billingAddress, invoiceLines);

        dbContext.SalesInvoices.Add(invoice);
        await dbContext.SaveChangesAsync();
    }

    public async Task<SalesOrder> CreateSalesOrderAsync(Guid? customerId, Guid? quotationId)
    {
        string salesOrderName = typeof(SalesOrder).Name;
        NumberSequence? numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(SalesOrder).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(SalesOrder).Name} not configured.");

        int salesOrderCount = await dbContext.SalesOrders.CountAsync();
        string salesOrderNumber = numberSequence.GenerateSequence(salesOrderCount);

        Address shipToAddress = null!;

        if (customerId.HasValue)
        {
            Customer? customer = await dbContext.Customers.FindAsync(customerId.Value);
            shipToAddress = customer?.BillingAddress!;
        }

        if (shipToAddress == null)
        {
            throw new Exception("Shipping address cannot be null.");
        }

        SalesOrder salesOrder = new(salesOrderNumber, customerId, quotationId, shipToAddress, []);

        salesOrder = dbContext.SalesOrders.Add(salesOrder).Entity;
        await dbContext.SaveChangesAsync();

        return salesOrder;

    }
}



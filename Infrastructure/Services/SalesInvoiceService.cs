using Edgias.MurimiOS.Domain.Interfaces;
using Edgias.MurimiOS.Infrastructure.Data;

namespace Edgias.MurimiOS.Infrastructure.Services;

public class SalesInvoiceService(MurimiOSDbContext dbContext) : ISalesInvoiceService
{
    public async Task AddInvoiceItemAsync(Guid invoiceId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1)
    {
        SalesInvoice? salesInvoice = await dbContext.SalesInvoices.FindAsync(invoiceId) ??
            throw new ArgumentNullException($"Sales Invoice with id {invoiceId} cannot be found.");

        Item? item = await dbContext.Items.FindAsync(itemId) ?? throw new ArgumentNullException($"Item with id {itemId} cannot be found.");

        Domain.Entities.SalesInvoiceAggregate.InvoicedItem invoicedItem = new(item.Id, item.Name, item.Description);

        PriceList? priceList = await dbContext.PriceLists.FindAsync(priceListId) ?? throw new ArgumentNullException($"Price list with id {priceListId} cannot be found.");

        SalesInvoiceItem salesInvoiceItem = new(invoicedItem, priceList.UnitPrice, units, taxId);

        salesInvoice!.AddItem(salesInvoiceItem);

        dbContext.Update(salesInvoice);
        await dbContext.SaveChangesAsync();

    }

    public async Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, List<SalesInvoiceItem> invoiceItems,
        Guid? customerId, Guid? salesOrderId)
    {
        NumberSequence? numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(SalesInvoice).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(SalesInvoice).Name} not configured.");

        int invoiceCount = await dbContext.SalesInvoices.CountAsync();

        string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

        string invoiceNotes = string.Empty;

        SalesInvoiceNote? salesInvoiceNote = await dbContext.SalesInvoiceNotes.FindAsync(invoiceNoteId!.Value);
        invoiceNotes = salesInvoiceNote != null ? salesInvoiceNote.Description : default!;

        Address billingAddress;
        Customer? customer = await dbContext.Customers.FindAsync(customerId!.Value) ??
            throw new ArgumentNullException($"Customer with id {customerId!.Value} cannot be found.");
        billingAddress = customer.BillingAddress;

        SalesInvoice invoice = new(invoiceNumber, dueDate, invoiceNotes, customerId, salesOrderId, billingAddress, invoiceItems);

        invoice = dbContext.SalesInvoices.Add(invoice).Entity;
        await dbContext.SaveChangesAsync();

        return invoice;
    }

    public async Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, Guid? customerId, Guid? salesOrderId)
    {
        NumberSequence? numberSequence = await dbContext.NumberSequences.FirstOrDefaultAsync(ns => ns.Entity == typeof(SalesInvoice).Name) ??
            throw new ArgumentNullException($"Number sequence for {typeof(SalesInvoice).Name} not configured.");

        int invoiceCount = await dbContext.SalesInvoices.CountAsync();

        string invoiceNumber = numberSequence.GenerateSequence(invoiceCount);

        string invoiceNotes = string.Empty;

        SalesInvoiceNote? salesInvoiceNote = await dbContext.SalesInvoiceNotes.FindAsync(invoiceNoteId!.Value);
        invoiceNotes = salesInvoiceNote != null ? salesInvoiceNote.Description : default!;

        Address billingAddress;
        Customer? customer = await dbContext.Customers.FindAsync(customerId!.Value) ??
            throw new ArgumentNullException($"Customer with id {customerId!.Value} cannot be found.");
        billingAddress = customer.BillingAddress;

        SalesInvoice invoice = new(invoiceNumber, dueDate, invoiceNotes, customerId, salesOrderId, billingAddress);

        invoice = dbContext.SalesInvoices.Add(invoice).Entity;
        await dbContext.SaveChangesAsync();

        return invoice;
    }
}



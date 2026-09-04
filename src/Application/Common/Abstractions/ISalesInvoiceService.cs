using EdgIQ.MurimiOS.Domain.SalesInvoice;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using SalesInvoice = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoice;
using SalesInvoiceItem = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoiceItem;

public interface ISalesInvoiceService
{
    Task AddInvoiceItemAsync(Guid invoiceId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1);

    Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, Guid? customerId, Guid? salesOrderId);

    Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, List<SalesInvoiceItem> invoiceItems,
        Guid? customerId, Guid? salesOrderId);
}

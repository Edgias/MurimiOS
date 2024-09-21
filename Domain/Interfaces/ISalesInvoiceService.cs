using Edgias.MurimiOS.Domain.Entities.SalesInvoiceAggregate;

namespace Edgias.MurimiOS.Domain.Interfaces;

public interface ISalesInvoiceService
{
    Task AddInvoiceItemAsync(Guid invoiceId, Guid itemId, Guid priceListId, Guid? taxId, int units = 1);

    Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, Guid? customerId, Guid? salesOrderId);

    Task<SalesInvoice> CreateInvoiceAsync(DateTime dueDate, Guid? invoiceNoteId, List<SalesInvoiceItem> invoiceItems,
        Guid? customerId, Guid? salesOrderId);
}

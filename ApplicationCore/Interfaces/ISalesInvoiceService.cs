using Murimi.ApplicationCore.Entities.SalesInvoiceAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murimi.ApplicationCore.Interfaces
{
    public interface ISalesInvoiceService
    {
        Task AddInvoiceItemAsync(Guid invoiceId, Guid itemId, Guid priceListId, Guid? taxId, string userId, int units = 1);

        Task<SalesInvoice> CreateInvoiceAsync(DateTimeOffset dueDate, Guid? invoiceNoteId, Guid? customerId, Guid? salesOrderId, string userId);

        Task<SalesInvoice> CreateInvoiceAsync(DateTimeOffset dueDate, Guid? invoiceNoteId, List<SalesInvoiceItem> invoiceItems,
            Guid? customerId, Guid? salesOrderId, string userId);
    }
}

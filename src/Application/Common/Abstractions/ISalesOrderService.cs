using EdgIQ.MurimiOS.Domain.SalesOrder;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using SalesOrder = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrder;

public interface ISalesOrderService
{
    Task AddSalesOrderItemAsync(Guid salesOrderId, Guid itemId, Guid priceListId, int units = 1);

    Task ConvertToInvoiceAsync(Guid salesOrderId);

    Task<SalesOrder> CreateSalesOrderAsync(Guid? customerd, Guid? quotationId);
}

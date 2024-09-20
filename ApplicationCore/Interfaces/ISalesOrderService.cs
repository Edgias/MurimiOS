using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;
using System;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.Interfaces
{
    public interface ISalesOrderService
    {
        Task AddSalesOrderItemAsync(Guid salesOrderId, Guid itemId, Guid priceListId, int units = 1);

        Task ConvertToInvoiceAsync(Guid salesOrderId);

        Task<SalesOrder> CreateSalesOrderAsync(Guid? customerd, Guid? quotationId);
    }
}

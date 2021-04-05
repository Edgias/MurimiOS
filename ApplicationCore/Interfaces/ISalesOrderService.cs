using Edgias.Agrik.ApplicationCore.Entities.SalesOrderAggregate;
using System;
using System.Threading.Tasks;

namespace Edgias.Agrik.ApplicationCore.Interfaces
{
    public interface ISalesOrderService
    {
        Task AddSalesOrderItemAsync(Guid salesOrderId, Guid itemId, Guid priceListId, string userId, int units = 1);

        Task ConvertToInvoiceAsync(Guid salesOrderId, string userId);

        Task<SalesOrder> CreateSalesOrderAsync(Guid? customerd, Guid? quotationId, string userId);
    }
}

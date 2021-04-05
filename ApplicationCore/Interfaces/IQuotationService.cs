using Murimi.ApplicationCore.Entities.QuotationAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murimi.ApplicationCore.Interfaces
{
    public interface IQuotationService
    {
        Task AddQuotationItemAsync(Guid quotationId, Guid itemId, Guid priceListId, Guid? taxId, string userId, int units = 1);

        Task ConvertToSalesOrderAsync(Guid quotationId, string userId);

        Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime validUntil, Guid? customerId, string userId);

        Task<Quotation> CreateQuotationAsync(DateTime quotationDate, DateTime validUntil, List<QuotationItem> quotationItems, Guid? customerId, string userId);

    }
}
